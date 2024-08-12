import { AfterViewInit, ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
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
import { ModuleGroup } from 'src/services/interfaces/moduleGroup.model';
import { LookupService } from 'src/services/lookup.service';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { ModuleGroupService } from 'src/services/modulegroup.service';
import { AlertService } from 'src/services/alert.service';
import { ModuleGroupCreateUpdateComponent } from './module-group-create-update/module-group-create-update.component';
import { Role } from 'src/services/interfaces/role.model';
import { Customer } from 'src/services/interfaces/customer.model';
import { Location } from 'src/services/interfaces/models';
import { RoleService } from 'src/services/role.service';
import { CustomerService } from 'src/services/customer.service';
import { LocationService } from 'src/services/location.service';
import { AuthService } from 'src/services/auth.service';
import { Router } from '@angular/router';
import { UserService } from 'src/services/user.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User } from 'src/services/interfaces/models';

@UntilDestroy()
@Component({
  selector: 'vex-module-group-table',
  templateUrl: './module-group-table.component.html',
  styleUrls: ['./module-group-table.component.scss'],
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
export class ModuleGroupTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<ModuleGroup>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Code', property: 'code', type: 'text', visible: true },
    { label: 'Description', property: 'description', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');

  subject$: ReplaySubject<ModuleGroup[]> = new ReplaySubject<ModuleGroup[]>(1);
  data$: Observable<ModuleGroup[]> = this.subject$.asObservable();


  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  dataSource: MatTableDataSource<ModuleGroup> | null;
  selection = new SelectionModel<ModuleGroup>(true, []);
  searchCtrl = new UntypedFormControl();

  roleList: Role[];
  customerList: Customer[];
  locationList = {} as Location[]
  userTypeList: Lookup[];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private moduleGroupService: ModuleGroupService,
    private roleService: RoleService,
    private customerService: CustomerService,
    private locationService: LocationService,
    private userService: UserService,
    private lookupService: LookupService,
    private authService: AuthService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ModuleManagement);
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
      filter<ModuleGroup[]>(Boolean)
    ).subscribe(moduleGroups => {
      this.dataSource.data = moduleGroups;
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
    this.moduleGroupService.getModuleGroups().subscribe((result: ModuleGroup[]) => (this.subject$.next(result)));
  }

  getLookups() {
    this.roleService.getRoles().subscribe((result: Role[]) => (this.roleList = result));
    this.customerService.getCustomers().subscribe((result: Customer[]) => (this.customerList = result));
    this.locationService.getLocations().subscribe((result = {} as Location[]) => (this.locationList = result));
    this.lookupService.getUserTypes().subscribe((result: Lookup[]) => (this.userTypeList = result));
  }

  createModuleGroup() {
    this.dialog.open(ModuleGroupCreateUpdateComponent, {
      height: '90%',
      width: '50%',
    }).afterClosed().subscribe((moduleGroup: ModuleGroup) => {
      if (moduleGroup) {
        this.moduleGroupService.createModuleGroup(moduleGroup).subscribe(result => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("ModuleGroup", "Create");
          }
          else {
            this.alertService.failNotification("ModuleGroup", "Create");
          }
        });
      }
    });
  }

  updateModuleGroup(moduleGroup: ModuleGroup) {
    this.dialog.open(ModuleGroupCreateUpdateComponent, {
      height: '90%',
      width: '50%',
      data: moduleGroup
    }).afterClosed().subscribe(updatedModuleGroup => {
      if (updatedModuleGroup) {
        this.moduleGroupService.updateModuleGroup(updatedModuleGroup).subscribe((result: ModuleGroup[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("ModuleGroup", "Update");
          }
          else this.alertService.failNotification("ModuleGroup", "Update");
        });
      }
    });
  }

  deleteModuleGroup(user: ModuleGroup) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (user) {
      // this.userService.deleteModuleGroup([user]).subscribe((result: ModuleGroup[]) => (this.subject$.next(result)));
    }

    // this.users.splice(this.users.findIndex((existingModuleGroup) => existingModuleGroup.id === user.id), 1);
    // this.selection.deselect(user);
    // this.subject$.next(this.users);
  }

  deleteModuleGroups(users: ModuleGroup[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (users.length > 0) {
      // this.userService.deleteModuleGroup(users).subscribe((result: ModuleGroup[]) => (this.subject$.next(result)));
    }

    // users.forEach(c => this.deleteModuleGroup(c));
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

  getRoleName(value: number) {
    if (this.roleList) {
      let entity = this.roleList.find(e => e.id === value);
      return entity ? entity.name : '';
    }
    return '';
  }

  getCustomerName(value: number) {
    if (this.customerList) {
      let entity = this.customerList.find(e => e.id === value);
      return entity ? entity.customerName : '';
    }
    return '';
  }

  getLocationName(value: number) {
    if (this.locationList) {
      let entity = this.locationList.find(e => e.id === value);
      return entity ? entity.locationName : '';
    }
    return '';
  }

  getUserTypeName(value: number) {
    if (this.userTypeList) {
      let entity = this.userTypeList.find(e => e.id === value + 1);
      return entity ? entity.name : '';
    }
    return '';
  }

  getBooleanText(value: boolean) {
    return value === false ? 'False' : 'True';
  }
}
