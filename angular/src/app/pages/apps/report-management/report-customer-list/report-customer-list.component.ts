import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { CustomerService } from 'src/services/customer.service';
import { CustomerDTO, CustomerDTOPaginatedListDTO, PaymentTerm, User, Zone } from 'src/services/interfaces/models';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ZoneService } from 'src/services/zone.service';
import { LookupService } from 'src/services/lookup.service';

@UntilDestroy()
@Component({
  selector: 'vex-report-customer-list',
  templateUrl: './report-customer-list.component.html',
  styleUrls: ['./report-customer-list.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class ReportCustomerListComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  imageDefault = "assets/img/pfw_logo_sm.png";

  paymentTermList: PaymentTerm[] = [];
  // zones: Zone[];
  dataSource: MatTableDataSource<CustomerDTO> | null;
  selection = new SelectionModel<CustomerDTO>(true, []);
  searchCtrl = new UntypedFormControl()
  
  pageSize: number = 100;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100, 500];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';
  searchPaymentTermId: number = 0;
  searchState: string = '';
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  columns: TableColumn<CustomerDTO>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Customer/Business', property: 'customerName', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Account Number', property: 'accountNumber', type: 'text', visible: true },
    { label: 'Phone Number', property: 'phoneNumber', type: 'text', visible: true },
    { label: 'Payment Term', property: 'paymentTermId', type: 'number', visible: true },
    { label: 'Address Line 1', property: 'addressLine1', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any, 
    private dialogRef: MatDialogRef<ReportCustomerListComponent>,
    private router: Router,
    private customerService: CustomerService,
    private lookupService: LookupService,
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
    this.getPaginatedCustomersList();
    this.lookupService.getPaymentTerms().subscribe((result: PaymentTerm[]) => (this.paymentTermList = result));
  }

  getPaginatedCustomersList() {
    this.alertService.showBlockUI('Loading Customers...');
    if (this.defaults.stateFilter !== undefined) this.searchState = this.defaults.stateFilter != 0 ? this.defaults.stateFilter : '';
    if (this.defaults.paymentTermId !== undefined) this.searchPaymentTermId = this.defaults.paymentTermId;
    if (this.defaults.customerFilter !== undefined) this.search = this.defaults.customerFilter.trim();
    if (!!this.search) this.search = this.search.trim();

    this.search = this.search.replace('&', "<--->");

    this.customerService.getReportCustomersListPaginated(this.pageSize, this.pageIndex, "CustomerName", "ASC", this.search, this.searchPaymentTermId, this.searchState).subscribe((result: CustomerDTOPaginatedListDTO) => {
      let customers = result.data;
      this.dataSource.data = customers;
      this.dataCount = result.recordCount;

      if (this.defaults.customerFilter !== undefined) {
        this.searchCtrl.setValue(this.defaults.customerFilter.trim());
        this.defaults.customerFilter = undefined;
      }
      this.alertService.hideBlockUI();
    });
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
    this.search = value;
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

  selectCustomer(customer: CustomerDTO){
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

  getPaymentTermName(id: number) {
    return this.paymentTermList.find(e => e.id === id).termName;
  }
}
