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
import { LookupService } from 'src/services/lookup.service';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { UserService } from 'src/services/user.service';
import { AlertService } from 'src/services/alert.service';
import { UserCreateUpdateComponent } from './user-create-update/user-create-update.component';
import { Role } from 'src/services/interfaces/role.model';
import { Customer } from 'src/services/interfaces/customer.model';
import { Location } from 'src/services/interfaces/models';
import { RoleService } from 'src/services/role.service';
import { CustomerService } from 'src/services/customer.service';
import { LocationService } from 'src/services/location.service';
import { AuthService } from 'src/services/auth.service';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User, UserDTO } from 'src/services/interfaces/models';

@UntilDestroy()
@Component({
  selector: 'vex-user-table',
  templateUrl: './user-table.component.html',
  styleUrls: ['./user-table.component.scss'],
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
export class UserTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<User>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'User Name', property: 'userName', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Email', property: 'email', type: 'text', visible: true },
    { label: 'User Type', property: 'isCustomerUser', type: 'text', visible: true },
    { label: 'Activated', property: 'isActivated', type: 'text', visible: true },
    { label: 'User Role', property: 'roleId', type: 'text', visible: true },
    { label: 'Customer/Business', property: 'customerId', type: 'text', visible: true },
    { label: 'Location', property: 'locationId', type: 'text', visible: true },
    { label: 'Change Password on Login', property: 'isChangePasswordOnLogin', type: 'text', visible: false },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');

  subject$: ReplaySubject<User[]> = new ReplaySubject<User[]>(1);
  data$: Observable<User[]> = this.subject$.asObservable();
  users: User[];

  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  dataSource: MatTableDataSource<User> | null;
  selection = new SelectionModel<User>(true, []);
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
    private roleService: RoleService,
    private customerService: CustomerService,
    private locationService: LocationService,
    private userService: UserService,
    private lookupService: LookupService,
    private authService: AuthService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.UserManagement);
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
      filter<User[]>(Boolean)
    ).subscribe(users => {
      this.users = users;
      this.dataSource.data = users;
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
    this.alertService.showBlockUI('Loading Users...');
    this.userService.getUsers().subscribe((result: User[]) => {
      this.subject$.next(result);
      this.alertService.hideBlockUI();
    });
  }

  getLookups() {
    this.alertService.showBlockUI('Loading Roles...');
    this.roleService.getRoles().subscribe((result: Role[]) => {
      this.roleList = result;
      this.alertService.hideBlockUI();
    });

    this.alertService.showBlockUI('Loading Customers...');
    this.customerService.getCustomers().subscribe((result: Customer[]) => {
      this.customerList = result;
      this.alertService.hideBlockUI();
    });

    this.alertService.showBlockUI('Loading Locations...');
    this.locationService.getLocations().subscribe((result = {} as Location[]) => {
      this.locationList = result;
      this.alertService.hideBlockUI();
    });

    this.alertService.showBlockUI('Loading User Types...');
    this.lookupService.getUserTypes().subscribe((result: Lookup[]) => {
      this.userTypeList = result;
      this.alertService.hideBlockUI();
    });
  }

  createUser() {
    this.dialog.open(UserCreateUpdateComponent, {
      height: '90%',
      width: '40%',
    }).afterClosed().subscribe((userdto: UserDTO) => {
      if (userdto) {
        this.authService.register(userdto).subscribe(result => {
          if (result.status === 200) {
            // Verify if Needed: this.sendUserRegistrationEmail(userdto);

            (this.subject$.next(result.users));
            this.alertService.successNotification("User", "Create");
          }
          else {
            this.alertService.failNotification("User", "Create");
          }
        });
      }
    });
  }

  updateUser(user: User) {
    this.dialog.open(UserCreateUpdateComponent, {
      height: '90%',
      width: '40%',
      data: user
    }).afterClosed().subscribe(updatedUser => {
      if (updatedUser) {
        this.userService.updateUser(updatedUser).subscribe((result: User[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("User", "Update");
          }
          else this.alertService.failNotification("User", "Update");
        });
      }
    });
  }

  deleteUser(user: User) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (user) {
      // this.userService.deleteUser([user]).subscribe((result: User[]) => (this.subject$.next(result)));
    }

    // this.users.splice(this.users.findIndex((existingUser) => existingUser.id === user.id), 1);
    // this.selection.deselect(user);
    // this.subject$.next(this.users);
  }

  deleteUsers(users: User[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (users.length > 0) {
      // this.userService.deleteUser(users).subscribe((result: User[]) => (this.subject$.next(result)));
    }

    // users.forEach(c => this.deleteUser(c));
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
    if (this.locationList && this.locationList.length > 0 && value !== null) {
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
