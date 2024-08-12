import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { OrderService } from 'src/services/order.service';
import { Order, OrderPaginatedListDTO, PaymentTerm, User, Zone } from 'src/services/interfaces/models';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ZoneService } from 'src/services/zone.service';
import { LookupService } from 'src/services/lookup.service';
import moment from 'moment';

@UntilDestroy()
@Component({
  selector: 'vex-report-invoice-list',
  templateUrl: './report-invoice-list.component.html',
  styleUrls: ['./report-invoice-list.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class ReportInvoiceListComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  imageDefault = "assets/img/pfw_logo_sm.png";

  paymentTermList: PaymentTerm[] = [];
  // zones: Zone[];
  dataSource: MatTableDataSource<Order> | null;
  selection = new SelectionModel<Order>(true, []);
  searchCtrl = new UntypedFormControl()
  deliveryRouteList = [ {id: 1, code: 'AM'}, {id: 2, code: 'PM'} ];
  
  pageSize: number = 100;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';
  searchPaymentTermId: number = 0;
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  columns: TableColumn<Order>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Order Number', property: 'orderNumber', type: 'text', visible: true },
    { label: 'Customer/Business', property: 'customerName', type: 'text', visible: true},
    { label: 'Invoice Number', property: 'invoiceNumber', type: 'text', visible: true },
    { label: 'Delivery Date', property: 'deliveryDate', type: 'text', visible: true },
    { label: 'Delivery Route', property: 'deliveryRoute', type: 'number', visible: true },
    { label: 'State', property: 'billState', type: 'number', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any, 
    private dialogRef: MatDialogRef<ReportInvoiceListComponent>,
    private router: Router,
    private orderService: OrderService,
    private zoneService: ZoneService,
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
    this.getPaginatedInvoicesList();
    //this.lookupService.getPaymentTerms().subscribe((result: PaymentTerm[]) => (this.paymentTermList = result));
  }

  getPaginatedInvoicesList() {
    this.alertService.showBlockUI('Loading Invoices...');
    if (this.defaults.paymentTermId !== undefined) this.searchPaymentTermId = this.defaults.paymentTermId;
    if (this.defaults.orderFilter !== undefined) this.search = this.defaults.orderFilter.trim();
    if (!!this.search) this.search = this.search.trim();

    this.search = this.search.replace('&', "<--->");

    this.orderService.getReportOrdersListPaginated(this.defaults.deliveryDate, this.pageSize, this.pageIndex, "OrderNumber", "ASC", this.search, this.defaults.state, this.defaults.deliveryRoute).subscribe((result: OrderPaginatedListDTO) => {
      let orders = result.data;
      this.dataSource.data = orders;
      this.dataCount = result.recordCount;

      if (this.defaults.orderFilter !== undefined) {
        this.searchCtrl.setValue(this.defaults.orderFilter.trim());
        this.defaults.orderFilter = undefined;
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
    //this.dataSource.filter = value;
    this.search = value;
    //this.getPaginatedProducts();
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getPaginatedInvoicesList();
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

  selectInvoice(order: Order){
    // if (order.isHoldAccount) {
    //   return this.alertService.selectAccountOnHoldNotification('Customer', order.orderName);
    // }
    this.selection.toggle(order);
  }

  selectInvoices(event) {
    const button = (event.srcElement.disabled === undefined) ? event.srcElement.parentElement : event.srcElement;
    button.setAttribute('disabled', true);
    setTimeout(function () {
      button.removeAttribute('disabled');
    }, 10000);

    let orders: Order[] = [];
    this.selection.selected.forEach(order => {
      orders.push(order);
    });

    this.dialogRef.close(orders);
  }

  onPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.getPaginatedInvoicesList();
  }

  searchInvoices() {
    this.getPaginatedInvoicesList();
  }

  getPaymentTermName(id: number) {
    return this.paymentTermList.find(e => e.id === id).termName;
  }

  getRoute(route: number) {
    if (route) {
      let result = this.deliveryRouteList.find(e => e.id === route);
      return (result) ? result.code : '';
    }

    return '';
  }

  formatDateOnly(date: moment.Moment) {
    return moment(date).format('MM/DD/YYYY');
  }
}
