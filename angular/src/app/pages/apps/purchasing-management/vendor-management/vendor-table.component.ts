import { AfterViewInit, ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
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
import { VendorService } from 'src/services/vendor.service';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { Vendor, User } from 'src/services/interfaces/models';
import moment from 'moment';
import { VendorCreateUpdateComponent } from './vendor-create-update/vendor-create-update.component';

@UntilDestroy()
@Component({
  selector: 'vex-vendor-table',
  templateUrl: './vendor-table.component.html',
  styleUrls: ['./vendor-table.component.scss'],
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
export class VendorTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<Vendor>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Vendor Name', property: 'vendorName', type: 'text', visible: true },
    { label: 'Vendor Code', property: 'vendorCode', type: 'text', visible: true },
    { label: 'Contact Name', property: 'contactName', type: 'text', visible: true },
    { label: 'Phone Number', property: 'phoneNumber', type: 'text', visible: true },
    { label: 'Email', property: 'email', type: 'text', visible: true },
    { label: 'Address', property: 'addressLine1', type: 'text', visible: true },
    { label: 'City', property: 'city', type: 'text', visible: true },
    { label: 'State', property: 'state', type: 'text', visible: true },
    { label: 'CA Vendor', property: 'isCAVendor', type: 'image', visible: true },
    { label: 'CA Rank', property: 'caRank', type: 'text', visible: true },
    { label: 'NV Vendor', property: 'isNVVendor', type: 'image', visible: true },
    { label: 'NV Rank', property: 'nvRank', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');

  // subject$: ReplaySubject<Payment[]> = new ReplaySubject<Payment[]>(1);
  // data$: Observable<Payment[]> = this.subject$.asObservable();
  // inventories: Payment[];

  pageSize: number = 10;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';

  dataSource: MatTableDataSource<Vendor> | null;
  selection = new SelectionModel<Vendor>(true, []);
  searchCtrl = new UntypedFormControl();

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  isShowInactive: boolean = true;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private vendorService: VendorService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.VendorManagement);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
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

    this.dataSource = new MatTableDataSource();
    this.getData();
    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  getData() {
    this.getVendors();
  }

  getVendors() {
    this.alertService.showBlockUI('Loading Vendors...');
    if (!!this.search) this.search = this.search.trim();
    this.vendorService.getVendors().subscribe((result: Vendor[]) => {
      this.dataSource.data = result;
      this.alertService.hideBlockUI();
    });
  }

  createVendor() {
    this.dialog.open(VendorCreateUpdateComponent, {
      height: 'auto',
      width: '30%',
    }).afterClosed().subscribe((vendor: Vendor) => {
      if (vendor) {
        this.vendorService.createVendor(vendor).subscribe((result: Vendor[]) => {
          if (result) {
            this.dataSource.data = result;
            this.alertService.successNotification("Vendor", "Create");
          }
          else {
            this.alertService.failNotification("Vendor", "Create");
          }
        });
      }
    });
  }

  updateVendor(vendor: Vendor) {
    this.dialog.open(VendorCreateUpdateComponent, {
      height: 'auto',
      width: '30%',
      data: vendor
    }).afterClosed().subscribe(updatedVendor => {
      if (updatedVendor) {
        this.vendorService.updateVendor(updatedVendor).subscribe((result: Vendor[]) => {
          if (result) {
            this.dataSource.data = result;
            this.alertService.successNotification("Vendor", "Update");
          }
          else this.alertService.failNotification("Vendor", "Update");
        });
      }
    });
  }

  deleteVendor(vendor: Vendor) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (vendor) {
      // this.userService.deleteVendor([user]).subscribe((result: Vendor[]) => (this.subject$.next(result)));
    }

    // this.inventories.splice(this.inventories.findIndex((existingVendor) => existingVendor.id === user.id), 1);
    // this.selection.deselect(user);
    // this.subject$.next(this.inventories);
  }

  deleteVendors(inventories: Vendor[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deleteVendor(inventories).subscribe((result: Vendor[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deleteVendor(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    //this.dataSource.filter = value;
    this.search = value;
    //this.getPaginatedVendors();
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getVendors();
    }
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

  onPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    //this.dataCount = event.length;
    this.getVendors();
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
    //this.getPaginatedTenantLocations();
  }

  searchVendors() {
    //this.search = this.searchCtrl.value.trim();
    this.getVendors();
  }

  showInactiveVendors() {
    this.getData();
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }
}
