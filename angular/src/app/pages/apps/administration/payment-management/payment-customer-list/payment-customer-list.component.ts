import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { CustomerService } from 'src/services/customer.service';
import { CustomerDTO, CustomerDTOPaginatedListDTO, User, Zone } from 'src/services/interfaces/models';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ZoneService } from 'src/services/zone.service';

@UntilDestroy()
@Component({
  selector: 'vex-payment-customer-list',
  templateUrl: './payment-customer-list.component.html',
  styleUrls: ['./payment-customer-list.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class PaymentCustomerListComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  imageDefault = "assets/img/pfw_logo_sm.png";

  zones: Zone[];
  dataSource: MatTableDataSource<CustomerDTO> | null;
  selection = new SelectionModel<CustomerDTO>(true, []);
  searchCtrl = new UntypedFormControl()
  
  pageSize: number = 10;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  columns: TableColumn<CustomerDTO>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Customer/Business', property: 'customerName', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Account Number', property: 'accountNumber', type: 'text', visible: true },
    { label: 'Phone Number', property: 'phoneNumber', type: 'text', visible: true },
    { label: 'Address Line 1', property: 'addressLine1', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any, 
    private dialogRef: MatDialogRef<PaymentCustomerListComponent>,
    private router: Router,
    private customerService: CustomerService,
    private zoneService: ZoneService,
    private alertService: AlertService) { 
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit(): void {
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

  getData() {
    // this.zoneService.getZones().subscribe((result: Zone[]) => {
    //   if (result) {
    //     this.zones = result;
    //     this.getPaginatedCustomersList();
    //   }
    // });
    this.getPaginatedCustomersList();
  }

  getPaginatedCustomersList() {
    this.alertService.showBlockUI('Loading Customers...');
    if (this.defaults.customerFilter !== undefined) this.search = this.defaults.customerFilter.trim();
    if (!!this.search) this.search = this.search.trim();

    this.search = this.search.replace('&', "<--->");

    this.customerService.getCustomersListPaginated(this.pageSize, this.pageIndex, "CustomerName", "ASC", this.search).subscribe((result: CustomerDTOPaginatedListDTO) => {
      let customers = result.data;
      
      // customers.forEach(e => {
      //   let zone = this.zones.find(z => z.zipCode == e.zipCode);
      //   e.zone = zone != undefined ? zone.binCode : '';
      // });

      this.dataSource.data = customers;
      this.dataCount = result.recordCount;

      if (this.defaults.customerFilter !== undefined) {
        this.searchCtrl.setValue(this.defaults.customerFilter.trim());
        this.defaults.customerFilter = undefined;
      }
      this.alertService.hideBlockUI();
    });
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  
  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    //this.dataSource.filter = value;
    this.search = value;
    //this.getPaginatedProducts();
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getPaginatedCustomersList();
    }
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
  }
  
  selectCustomer(customer: CustomerDTO){
    if (customer.isHoldAccount) {
      return this.alertService.selectAccountOnHoldNotification('Customer', customer.customerName);
      
    }

    this.selection.toggle(customer);
  }

  selectCustomers(event) {
    const button = (event.srcElement.disabled === undefined) ? event.srcElement.parentElement : event.srcElement;
    button.setAttribute('disabled', true);
    setTimeout(function () {
      button.removeAttribute('disabled');
    }, 10000);

    let customers: CustomerDTO[] = [];
    this.selection.selected.forEach(customer => {
      customers.push(customer);
    });

    this.dialogRef.close(customers);
  }

  onPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.getPaginatedCustomersList();
  }

  searchCustomers() {
    this.getPaginatedCustomersList();
  }
}
