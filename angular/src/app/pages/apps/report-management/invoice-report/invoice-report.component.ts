import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SelectionModel } from '@angular/cdk/collections';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { UntilDestroy } from '@ngneat/until-destroy';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { ModuleCode, StoreLocation, UserPermission } from 'src/static-data/enums/enums';
import { CustomerDTO, Order, OrderDetail, PaymentTerm, User } from 'src/services/interfaces/models';
import moment from 'moment';
import { MatDialog } from '@angular/material/dialog';
import { LookupService } from 'src/services/lookup.service';
import { ReportService } from 'src/services/report.service';
import { ReportInvoiceListComponent } from '../report-invoice-list/report-invoice-list.component';

@UntilDestroy()
@Component({
  selector: 'vex-invoice-report',
  templateUrl: './invoice-report.component.html',
  styleUrls: ['./invoice-report.component.scss'],
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

export class InvoiceReportComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('location', { static: false }) location: ElementRef;
  @ViewChild('product', { static: false }) product: ElementRef;

  @Input()
  columns: TableColumn<CustomerDTO>[] = [
    { label: 'Order Number', property: 'orderNumber', type: 'text', visible: true },
    { label: 'Customer/Business', property: 'customerName', type: 'text', visible: true},
    { label: 'Invoice Number', property: 'invoiceNumber', type: 'text', visible: true },
    { label: 'Delivery Date', property: 'deliveryDate', type: 'text', visible: true },
    { label: 'Delivery Route', property: 'deliveryRoute', type: 'number', visible: true },
    { label: 'State', property: 'billState', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  form: UntypedFormGroup;
  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');
  pageSize: number = 10;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';
  invoiceDataList = [];
  selectedInvoiceList: Order[] = [];
  paymentTermList: PaymentTerm[] = [];

  //locationList = Object.values(StoreLocation);
  locationList = [ {id: 'CA', code: 'California'}, {id: 'NV', code: 'Nevada'} ];
  deliveryRouteList = [ {id: 1, code: 'AM'}, {id: 2, code: 'PM'} ];
  dataSource: MatTableDataSource<Order> | null;
  selection = new SelectionModel<Order>(true, []);
  locationCtrl = new UntypedFormControl();
  deliveryRouteCtrl = new UntypedFormControl();
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  isShowInactive: boolean = true;
  todayDate: Date = new Date();

  // returnReasonList = 
  // [
  //   { code:1, name:'Damaged' },
  //   { code:2, name: 'Wrong Part Delivered' }, // -  Required : enter why its wrong
  //   { code:3, name: 'Car Totaled' },
  //   { code:4, name: 'Defective' },
  //   { code:5, name: 'Customer Cancel' },
  //   { code:6, name: 'Price and Billing Adjustment' },
  //   { code:7, name: 'Manager Approval'} // - Required : enter manager/supervisor name
  // ]

  returnReasonList = 
  [
    { code:1, name:'Damaged' },
    { code:2, name: 'Agent Sold Wrong' }, // -  Required : enter why its wrong
    { code:3, name: 'Car Totaled' },
    { code:4, name: 'Defective' },
    { code:5, name: 'Customer Do Not Need' },
    { code:6, name: 'Price and Billing Adjustment' },
    { code:7, name: 'Manager Approval' }, // - Required : enter manager/supervisor name
    { code:8, name: 'Price Too High' },
    { code:9, name: 'Got Somewhere Else' },
    { code:10, name: 'Customer Ordered Wrong' }
  ]

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private lookupService: LookupService,
    private reportService: ReportService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.InvoiceReport);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.locationCtrl.setValue('CA');
    this.deliveryRouteCtrl.setValue(0);
    this.dataSource = new MatTableDataSource();
    this.getData();
    this.initializeFormGroup();
  }

  initializeFormGroup() {
    this.form = this.fb.group({
      deliveryDate: [this.todayDate],
    });
  }

  openInvoiceList() {
    let deliveryDate = moment(new Date(this.form.value.deliveryDate)).format('MM/DD/YYYY');
    this.dialog.open(ReportInvoiceListComponent, {
      height: '100%',
      width: '100%',
      data: { deliveryDate: deliveryDate, state: this.locationCtrl.value, deliveryRoute: this.deliveryRouteCtrl.value }
    }).afterClosed().subscribe((invoices: Order[]) => {
      if (invoices && invoices.length > 0) {
        invoices.forEach(e => {
          if (this.selectedInvoiceList.findIndex(c => c.id === e.id) === -1) {
            this.selectedInvoiceList.push(e);    
          }
        });

        this.dataSource.data = this.selectedInvoiceList;
        this.cd.detectChanges();
      }
    });
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  getData() {
    this.lookupService.getPaymentTerms().subscribe((result: PaymentTerm[]) => (this.paymentTermList = result));
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

  trackByProperty<T>(column: TableColumn<T>) {
    return column.property;
  }

  onPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatDateOnly(date: moment.Moment) {
    return moment(date).format('MM/DD/YYYY');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  getRoute(route: number) {
    if (route) {
      let result = this.deliveryRouteList.find(e => e.id === route);
      return (result) ? result.code : '';
    }

    return '';
  }

  deleteInvoice(invoice: Order) {
    this.alertService.removeNotification('Invoice').then(answer => {
      if (!answer.isConfirmed) return;
      this.selectedInvoiceList.splice(this.selectedInvoiceList.findIndex((e) => e.id === invoice.id), 1);
      this.dataSource.data = this.selectedInvoiceList;
      this.cd.detectChanges();
    });
  }

  getPaymentTermName(id: number) {
    return this.paymentTermList.find(e => e.id === id).termName;
  }

  clear() {
    this.form.get('deliveryDate').setValue(this.todayDate);
    this.locationCtrl.setValue('CA');
    this.deliveryRouteCtrl.setValue(0);
    this.selectedInvoiceList = [];
    this.dataSource.data = [];
    this.cd.detectChanges();
  }

  print() {
    this.invoiceDataList = [];
    this.dataSource.data.forEach(e => {
      let data = this.mapRowTodata(e);
      this.invoiceDataList.push(data);
    });

    setTimeout(() => {
      window.print();

      this.alertService.printSuccessNotification('Invoice').then(answer => {
        if (!answer.isConfirmed) return;
        let orderIds = this.dataSource.data.map(e => e.id);
        this.reportService.updatePrintedInvoice(orderIds).subscribe(result => {
          if (result) {
            this.clear();
          }
          else this.alertService.failNotification('Invoice Is Printed', 'Update');  
        });
      });
    }, 2000);
  }

  mapRowTodata(row: Order): any {
    let result =  {
      imagePath: row.billState === 'CA' ? 'assets/img/pfitwest.png' : 'assets/img/partsco.jpg',
      orderStatusId: row.orderStatusId,
      rgaType: row.rgaType === 1 ? 'INCOMING' : 'OUTGOING',
      rgaReason: row.rgaReason && row.rgaReason > 0 ? this.returnReasonList.find(e => e.code === row.rgaReason).name : '',
      rgaReasonNotes:   row.rgaReasonNotes,
      isCreditMemo: row.orderStatusId === 5,
      orderNumber: row.orderNumber,
      quoteNumber: row.quoteNumber,
      isQuote: row.isQuote,
      invoiceNumber: row.invoiceNumber,
      purchaseOrderNumber:row.purchaseOrderNumber,
      address: row.billState === 'CA' ? '3383 OLIVE AVE, SIGNAL HILL CA 90755' : '5151 W Oquendo Rd, Las Vegas, NV 89118',
      printDate: row.orderDate.toLocaleString(),
      phoneNumber: row.billState === 'CA' ? '310-956-4667' : '702-998-8888',
      website: row.billState === 'CA' ? 'PERFECTFITWEST.COM' : 'PartsCoInc.com',
      soldTo: row.customerName,
      soldToAddress: row.billAddress,
      soldToAddress2: row.billCity + ' ' + row.billState + ' ' + row.billZipCode, 
      shipTo: row.shipAddressName,
      shipToAddress: row.shipAddress, 
      shipToAddress2: row.shipCity + ' ' + row.shipState + ' ' + row.shipZipCode, 
      accountNumber: row.accountNumber,
      customerPhoneNumber: row.phoneNumber,
      customerTerms: row.paymentTermName,
      soldBy: row.createdBy,
      notes: row.orderedByNotes,
      orderType: (row.deliveryType === 1) ? 'DELIVERY' : (row.deliveryType === 2) ? 'PICK UP' : 'SHIPPING',
      orderedBy: row.orderedBy,
      orderedByPhone: row.orderedByPhoneNumber,
      zone: row.shipZone,
      vendorCode: '',
      deliveryDate: moment(row.deliveryDate).format('MM/DD/YYYY') + ' - ' + (row.deliveryRoute === 1 ? 'AM' : 'PM'),
      subTotal: Number(row.subTotalAmount).toFixed(2),
      tax: Number(row.totalTax).toFixed(2),
      total: Number(row.totalAmount).toFixed(2),
      staticText1: '20% RESTOCKING FEE AFTER 10 DAYS, NO RETURNS AFTER 30 DAYS',
      staticText2: '50% RESTOCKING FEE FOR NO BAG/BOX ITEMS',
      totalQuantity: row.orderDetails.length,
      lineItems: this.mapRowLineItems(row.orderDetails)
    }

    return result;
  }

  mapRowLineItems(orderDetails: OrderDetail[]): any {
    let result: LineItem[] = [];
    let ordDet = orderDetails.sort((a, b) => a.partNumber.localeCompare(b.partNumber));
    ordDet.forEach(e => {
      const lineItem = {} as LineItem;
      lineItem.quantity = e.orderQuantity;
      lineItem.partNumber =  e.partNumber; 
      lineItem.description = e.yearFrom + '-' + e.yearTo + ' ' + e.partDescription + ', ' + e.mainPartsLinkNumber + ', ' + ((e.onHandQuantity > 0 && e.stocks.filter(e => Number(e.quantity) > 0).length > 0) ? e.stocks.filter(e => Number(e.quantity) > 0)[0].location + ', STOCK' : (e.vendorPartNumber + ', ' + e.vendorCode));
      lineItem.lprice = Number(e.listPrice).toFixed(2);
      lineItem.price = Number(e.wholesalePrice).toFixed(2);
      lineItem.extPrice = Number(e.totalAmount).toFixed(2)
      result.push(lineItem);
    });
    return result;
  }

  getStops() {
    if (this.dataSource.data.length === 0) return '0';
    let stops = new Set(this.dataSource.data.map(x => x.customerName));
    return stops.size;
  }
}

export interface LineItem {
  quantity: number, 
  partNumber: string, 
  description: string, 
  lprice: string, 
  price: string, 
  extPrice: string
}