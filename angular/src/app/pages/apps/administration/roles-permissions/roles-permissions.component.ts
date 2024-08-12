import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { SelectionModel } from '@angular/cdk/collections';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { UntypedFormControl } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { Role } from 'src/services/interfaces/role.model';
import { RolesPermissionsCreateUpdateComponent } from './roles-permissions-create-update/roles-permissions-create-update.component';
import { LookupService } from 'src/services/lookup.service';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { RoleService } from 'src/services/role.service';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { Router } from '@angular/router';
import { User } from 'src/services/interfaces/models';

@UntilDestroy()
@Component({
  selector: 'vex-roles-permissions-table',
  templateUrl: './roles-permissions.component.html',
  styleUrls: ['./roles-permissions.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ],
  providers: [
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: {
        appearance: 'standard'
      } as MatFormFieldDefaultOptions
    }
  ]
})
export class RolesPermissionsComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<Role>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Code', property: 'code', type: 'text', visible: true },
    { label: 'User Type', property: 'userTypeId', type: 'text', visible: true },
    { label: 'Description', property: 'description', type: 'text', visible: true },
    // { label: 'Sort Order', property: 'sortOrder', type: 'text', visible: true},
    { label: 'Notes', property: 'notes', type: 'text', visible: false },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');

  subject$: ReplaySubject<Role[]> = new ReplaySubject<Role[]>(1);
  data$: Observable<Role[]> = this.subject$.asObservable();
  roles: Role[];

  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  dataSource: MatTableDataSource<Role> | null;
  selection = new SelectionModel<Role>(true, []);
  searchCtrl = new UntypedFormControl();

  userTypeList: Lookup[];
  accessTypeList: Lookup[];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private roleService: RoleService,
    private lookupService: LookupService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.RolesAndPermissions);
    this.access = modulePermission.accessTypeId;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.getData();
    this.getLookups();
    this.dataSource = new MatTableDataSource();

    this.data$.pipe(
      filter<Role[]>(Boolean)
    ).subscribe(roles => {
      this.roles = roles;
      this.dataSource.data = roles;
    });

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  getData() {
    this.alertService.showBlockUI('Loading Roles...');
    this.roleService.getRoles().subscribe((result: Role[]) => {
      this.subject$.next(result);
      this.alertService.hideBlockUI();
    });
  }

  getLookups() {
    this.lookupService.getAccessTypes().subscribe((result: Lookup[]) => (this.accessTypeList = result));
    this.lookupService.getUserTypes().subscribe((result: Lookup[]) => (this.userTypeList = result));
  }

  createRole() {
    this.dialog.open(RolesPermissionsCreateUpdateComponent, {
      height: '90%',
      width: '50%',
    }).afterClosed().subscribe((role: Role) => {
      if (role) {
        this.roleService.createRole(role).subscribe((result: Role[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Role", "Create");
          }
          else this.alertService.failNotification("Role", "Create");
        });
      }
    });
  }

  updateRole(role: Role) {
    this.dialog.open(RolesPermissionsCreateUpdateComponent, {
      height: '90%',
      width: '50%',
      data: role
    }).afterClosed().subscribe(updatedRole => {
      if (updatedRole) {
        this.roleService.updateRole(updatedRole).subscribe((result: Role[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Role", "Update");
          }
          else this.alertService.failNotification("Role", "Update");
        });
      }
    });
  }

  deleteRole(role: Role) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (role) {
      // this.roleService.deleteRole([role]).subscribe((result: Role[]) => (this.subject$.next(result)));
    }

    // this.roles.splice(this.roles.findIndex((existingRole) => existingRole.id === role.id), 1);
    // this.selection.deselect(role);
    // this.subject$.next(this.roles);
  }

  deleteRoles(roles: Role[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (roles.length > 0) {
      // this.roleService.deleteRole(roles).subscribe((result: Role[]) => (this.subject$.next(result)));
    }

    // roles.forEach(c => this.deleteRole(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.dataSource.filter = value;
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  getUserTypeName(value: number) {
    if (this.userTypeList) {
      let userType = this.userTypeList.find(ut => ut.id === value);
      return userType ? userType.name : '';
    }

    return '';
  }
}
