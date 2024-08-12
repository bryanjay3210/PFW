import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
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
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { AgingBalanceReportDTO, CustomerDTO, CustomerSalesAmountDTO, CustomerSalesFilterDTO, PaymentTerm, SalesData, StatementReportDTO, User } from 'src/services/interfaces/models';
import moment from 'moment';
import { MatDialog } from '@angular/material/dialog';
import { ReportService } from 'src/services/report.service';
import * as pdfMake from "pdfMake/build/pdfMake";
import * as pdfFonts from "pdfMake/build/vfs_fonts";
import { ToolbarNotificationsCustomerNoteDialog } from 'src/@vex/layout/toolbar/toolbar-notifications/toolbar-notifications-customer-note-dialog/toolbar-notifications-customer-note-dialog';
import { CustomerSalesSearchFilterComponent } from './customer-sales-search-filter/customer-sales-search-filter.component';
import { OrderService } from 'src/services/order.service';
(pdfMake as any).vfs = pdfFonts.pdfMake.vfs;

@UntilDestroy()
@Component({
  selector: 'vex-customer-sales-report',
  templateUrl: './customer-sales-report.component.html',
  styleUrls: ['./customer-sales-report.component.scss'],
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
export class CustomerSalesReportComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('location', { static: false }) location: ElementRef;
  @ViewChild('product', { static: false }) product: ElementRef;
  @Input()
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

  selectedCustomerList: CustomerDTO[] = [];
  paymentTermList: PaymentTerm[] = [];
  stateList: any = [{'id': 'CANV', 'name': 'CA & NV'}, {'id': 'CA', 'name': 'CA'}, {'id': 'NV', 'name': 'NV'}];
  customerList: CustomerDTO[] = [];
  statementReportList: StatementReportDTO[] = [];
  dataSource: MatTableDataSource<CustomerSalesAmountDTO> | null;
  ds2: CustomerSalesAmountDTO[] = [];
  selection = new SelectionModel<CustomerDTO>(true, []);
  stateCtrl = new UntypedFormControl();
  customerFilterCtrl = new UntypedFormControl();
  paymentTermCtrl = new UntypedFormControl();
  locationCtrl = new UntypedFormControl();
  productCtrl = new UntypedFormControl();
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  isShowInactive: boolean = true;

  todayDate: Date = new Date();
  cutOffDate: Date = new Date();
  dueDate: Date = new Date();
  
  progress: number;
  message: string;
  @Output() public onUploadFinished = new EventEmitter();
 
  columns: TableColumn<CustomerSalesAmountDTO>[] = [];
  reportData: CustomerSalesAmountDTO[] = [];
  filters: any;
  constructor(
    private router: Router,
    private dialog: MatDialog,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private orderService: OrderService,
    private reportService: ReportService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerSalesReport);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    // this.customerFilterCtrl.setValue('');
    this.stateCtrl.setValue('CANV');
    // this.paymentTermCtrl.setValue(0);
    // this.dueDate.setDate(this.cutOffDate.getDate() + 5);

    this.dataSource = new MatTableDataSource();
    this.initializeFormGroup();
  }

  initializeFormGroup() {
    this.form = this.fb.group({
      customerName: [''],
      accountNumber: [''],
      customerTerms: [''],
      statementDate: [this.cutOffDate],
      paymentDueDate: [this.dueDate],
      customerFilter: [''],
    });
  }

  initializeTableColumns(filters) {
    this.columns = undefined;
    this.cd.detectChanges();
    let newColumns: TableColumn<CustomerSalesAmountDTO>[] = [];
    newColumns = [
      { label: 'Account Name', property: 'customerName', type: 'text', visible: true},
      { label: 'Phone Number', property: 'phoneNumber', type: 'text', visible: true},
      { label: 'State', property: 'state', type: 'text', visible: true},
      { label: filters.col1FrDate === '' || filters.col1ToDate === '' ? 'Column 1' : this.getColumnHeader(1, filters.col1FrDate, filters.col1ToDate), property: 'column1', type: 'number', visible: true },
      { label: filters.col2FrDate === '' || filters.col2ToDate === '' ? 'Column 2' : this.getColumnHeader(2, filters.col2FrDate, filters.col2ToDate), property: 'column2', type: 'number', visible: true },
      { label: filters.col3FrDate === '' || filters.col3ToDate === '' ? 'Column 3' : this.getColumnHeader(3, filters.col3FrDate, filters.col3ToDate), property: 'column3', type: 'number', visible: true },
      { label: filters.col4FrDate === '' || filters.col4ToDate === '' ? 'Column 4' : this.getColumnHeader(4, filters.col4FrDate, filters.col4ToDate), property: 'column4', type: 'number', visible: true },
      { label: filters.col5FrDate === '' || filters.col5ToDate === '' ? 'Column 5' : this.getColumnHeader(5, filters.col5FrDate, filters.col5ToDate), property: 'column5', type: 'number', visible: true },
      { label: filters.col6FrDate === '' || filters.col6ToDate === '' ? 'Column 6' : this.getColumnHeader(6, filters.col6FrDate, filters.col6ToDate), property: 'column6', type: 'number', visible: true },
      { label: filters.col7FrDate === '' || filters.col7ToDate === '' ? 'Column 7' : this.getColumnHeader(7, filters.col7FrDate, filters.col7ToDate), property: 'column7', type: 'number', visible: true },
      { label: filters.col8FrDate === '' || filters.col8ToDate === '' ? 'Column 8' : this.getColumnHeader(8, filters.col8FrDate, filters.col8ToDate), property: 'column8', type: 'number', visible: true },
      { label: filters.col9FrDate === '' || filters.col9ToDate === '' ? 'Column 9' : this.getColumnHeader(9, filters.col9FrDate, filters.col9ToDate), property: 'column9', type: 'number', visible: true },
      { label: filters.col10FrDate === '' || filters.col10ToDate === '' ? 'Column 10' : this.getColumnHeader(10, filters.col10FrDate, filters.col10ToDate), property: 'column10', type: 'number', visible: true },
      { label: filters.col11FrDate === '' || filters.col11ToDate === '' ? 'Column 11' : this.getColumnHeader(11, filters.col11FrDate, filters.col11ToDate), property: 'column11', type: 'number', visible: true },
      { label: filters.col12FrDate === '' || filters.col12ToDate === '' ? 'Column 12' : this.getColumnHeader(12, filters.col12FrDate, filters.col12ToDate), property: 'column12', type: 'number', visible: true },
    ];

    this.columns = newColumns;
    this.cd.detectChanges();
  }

  getColumnHeader(colNo, frDate, toDate) {
    if (frDate && toDate) {
      return this.formatDateOnly(frDate) + ' - ' + this.formatDateOnly(toDate)
    }
    else {
      return 'Column ' + colNo;
    }
  }

  openSearchFilters() {
    this.dialog.open(CustomerSalesSearchFilterComponent, {
      data : this.filters
    }).afterClosed().subscribe((filters: CustomerSalesFilterDTO) => {
      if (filters) {
        this.filters = filters;
        this.initializeTableColumns(this.filters);
        this.getCustomerSales(filters);
        this.cd.detectChanges();
      }
    });
  }

  getCustomerSales(filter: CustomerSalesFilterDTO) {
    this.alertService.showBlockUI('Getting Customer Sales...');
    this.orderService.getCustomerSales(filter).subscribe(result => {
      if (result && result.length > 0) {
        this.reportData = result;
        this.dataSource.data = result;
        this.alertService.hideBlockUI();
        this.cd.detectChanges();
        
      }
      this.alertService.hideBlockUI();
    });
  }

  onDateChange() {
    this.cutOffDate = this.form.value.statementDate;
    this.dueDate.setDate(this.cutOffDate.getDate() + 5);
    this.form.get('paymentDueDate').setValue(this.dueDate);
  }

  filterReport() {
    let state = this.stateCtrl.value;
    let customer = this.customerFilterCtrl.value;
    
    let filteredData = {} as CustomerSalesAmountDTO[];
    filteredData = this.reportData;
    if (state === 'CA') {
      filteredData = this.reportData.filter(e => e.state === 'CA');
    }
    
    if (state === 'NV') {
      filteredData = this.reportData.filter(e => e.state === 'NV');
    }

    if (customer && customer.trim().length > 0) {
      filteredData = filteredData.filter(e => e.customerName.toLowerCase().includes(customer.toLowerCase()) || 
                                              e.phoneNumber.toLowerCase().includes(customer.toLowerCase()) || 
                                              e.accountNumber.toString().toLowerCase().includes(customer.toLowerCase()))
    }

    if (filteredData.length > 0 && filteredData.length < this.reportData.length) {
      // Create and Add Grand Totals
      let gt = {} as CustomerSalesAmountDTO;
      gt.accountNumber = 0;
      gt.customerId = 0;
      gt.customerName = 'GRAND TOTALS';
      gt.phoneNumber = '';
      gt.state = '';
      gt.column1 = {} as SalesData;
      gt.column2 = {} as SalesData;
      gt.column3 = {} as SalesData;
      gt.column4 = {} as SalesData;
      gt.column5 = {} as SalesData;
      gt.column6 = {} as SalesData;
      gt.column7 = {} as SalesData;
      gt.column8 = {} as SalesData;
      gt.column9 = {} as SalesData;
      gt.column10 = {} as SalesData;
      gt.column11 = {} as SalesData;
      gt.column12 = {} as SalesData;

      // Get Amount
      gt.column1.amount = filteredData.map(a => a.column1.amount).reduce(function(a, b) { return a + b; });
      gt.column2.amount = filteredData.map(a => a.column2.amount).reduce(function(a, b) { return a + b; });
      gt.column3.amount = filteredData.map(a => a.column3.amount).reduce(function(a, b) { return a + b; });
      gt.column4.amount = filteredData.map(a => a.column4.amount).reduce(function(a, b) { return a + b; });
      gt.column5.amount = filteredData.map(a => a.column5.amount).reduce(function(a, b) { return a + b; });
      gt.column6.amount = filteredData.map(a => a.column6.amount).reduce(function(a, b) { return a + b; });
      gt.column7.amount = filteredData.map(a => a.column7.amount).reduce(function(a, b) { return a + b; });
      gt.column8.amount = filteredData.map(a => a.column8.amount).reduce(function(a, b) { return a + b; });
      gt.column9.amount = filteredData.map(a => a.column9.amount).reduce(function(a, b) { return a + b; });
      gt.column10.amount = filteredData.map(a => a.column10.amount).reduce(function(a, b) { return a + b; });
      gt.column11.amount = filteredData.map(a => a.column11.amount).reduce(function(a, b) { return a + b; });
      gt.column12.amount = filteredData.map(a => a.column12.amount).reduce(function(a, b) { return a + b; });

      // Get Cost
      gt.column1.cost = filteredData.map(a => a.column1.cost).reduce(function(a, b) { return a + b; });
      gt.column2.cost = filteredData.map(a => a.column2.cost).reduce(function(a, b) { return a + b; });
      gt.column3.cost = filteredData.map(a => a.column3.cost).reduce(function(a, b) { return a + b; });
      gt.column4.cost = filteredData.map(a => a.column4.cost).reduce(function(a, b) { return a + b; });
      gt.column5.cost = filteredData.map(a => a.column5.cost).reduce(function(a, b) { return a + b; });
      gt.column6.cost = filteredData.map(a => a.column6.cost).reduce(function(a, b) { return a + b; });
      gt.column7.cost = filteredData.map(a => a.column7.cost).reduce(function(a, b) { return a + b; });
      gt.column8.cost = filteredData.map(a => a.column8.cost).reduce(function(a, b) { return a + b; });
      gt.column9.cost = filteredData.map(a => a.column9.cost).reduce(function(a, b) { return a + b; });
      gt.column10.cost = filteredData.map(a => a.column10.cost).reduce(function(a, b) { return a + b; });
      gt.column11.cost = filteredData.map(a => a.column11.cost).reduce(function(a, b) { return a + b; });
      gt.column12.cost = filteredData.map(a => a.column12.cost).reduce(function(a, b) { return a + b; });

      // Get Profit
      gt.column1.profit = gt.column1.amount - gt.column1.cost;
      gt.column2.profit = gt.column2.amount - gt.column2.cost;
      gt.column3.profit = gt.column3.amount - gt.column3.cost;
      gt.column4.profit = gt.column4.amount - gt.column4.cost;
      gt.column5.profit = gt.column5.amount - gt.column5.cost;
      gt.column6.profit = gt.column6.amount - gt.column6.cost;
      gt.column7.profit = gt.column7.amount - gt.column7.cost;
      gt.column8.profit = gt.column8.amount - gt.column8.cost;
      gt.column9.profit = gt.column9.amount - gt.column9.cost;
      gt.column10.profit = gt.column10.amount - gt.column10.cost;
      gt.column11.profit = gt.column11.amount - gt.column11.cost;
      gt.column12.profit = gt.column12.amount - gt.column12.cost;

      // Get Margin
      gt.column1.margin = gt.column1.amount !== 0 ? (gt.column1.profit / gt.column1.amount * 100) : 0
      gt.column2.margin = gt.column2.amount !== 0 ? (gt.column2.profit / gt.column2.amount * 100) : 0
      gt.column3.margin = gt.column3.amount !== 0 ? (gt.column3.profit / gt.column3.amount * 100) : 0
      gt.column4.margin = gt.column4.amount !== 0 ? (gt.column4.profit / gt.column4.amount * 100) : 0
      gt.column5.margin = gt.column5.amount !== 0 ? (gt.column5.profit / gt.column5.amount * 100) : 0
      gt.column6.margin = gt.column6.amount !== 0 ? (gt.column6.profit / gt.column6.amount * 100) : 0
      gt.column7.margin = gt.column7.amount !== 0 ? (gt.column7.profit / gt.column7.amount * 100) : 0
      gt.column8.margin = gt.column8.amount !== 0 ? (gt.column8.profit / gt.column8.amount * 100) : 0
      gt.column9.margin = gt.column9.amount !== 0 ? (gt.column9.profit / gt.column9.amount * 100) : 0
      gt.column10.margin = gt.column10.amount !== 0 ? (gt.column10.profit / gt.column10.amount * 100) : 0
      gt.column11.margin = gt.column11.amount !== 0 ? (gt.column11.profit / gt.column11.amount * 100) : 0
      gt.column12.margin = gt.column12.amount !== 0 ? (gt.column12.profit / gt.column12.amount * 100) : 0

      filteredData.unshift(gt);
    }
    

    this.dataSource.data = filteredData;
    this.cd.detectChanges();
  }

  clearFilters() {
    this.stateCtrl.setValue('CANV');
    this.customerFilterCtrl.setValue('');
    this.filterReport();
  }

  get visibleColumns() {
    if (this.columns) {
      return this.columns.filter(column => column.visible).map(column => column.property);
    }
  }

  ngAfterViewInit() {
    //this.dataSource.paginator = this.paginator;
    //this.dataSource.sort = this.sort;
  }

  onFilterChange(value: string) {
    // if (!this.dataSource) {
    //   return;
    // }

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

  formatDateOnly(orderDate: any) {
    if (!orderDate || orderDate == null) return '';
    return moment(orderDate).format('MM/DD/YYYY');
  }

  getSalesData(field: string, salesData: any) {
    if (!salesData || salesData === null) return '';
    switch (field) {
      case 'Amount': {
        if (salesData.amount && salesData.amount !== 0) {
          return (salesData.amount !== null) && salesData.amount !== 0 ? 'Amount: $' + salesData.amount.toFixed(2) : '';
        }
        break;
      }
      case 'Cost': {
        if (salesData.cost && salesData.cost !== 0) {
          return (salesData.cost !== null) && salesData.cost !== 0 ? 'Cost: $' + salesData.cost.toFixed(2) : '';
        }
        break;
      }
      case 'Profit': {
        if (salesData.profit && salesData.profit !== 0) {
          return (salesData.profit !== null) && salesData.profit !== 0 ? 'Profit: $' + salesData.profit.toFixed(2) : '';
        }
        break;
      }
      case 'Margin': {
        if (salesData.margin && salesData.margin !== 0) {
          return (salesData.margin !== null) && salesData.margin !== 0 ? 'Margin: ' + salesData.margin.toFixed(0) + '%' : '';
        }
        break;
      }
      default: {
        return '';
        break;
      }
    }
  }

  formatCurrency(amount: number) {
    return (amount) && amount !== 0 ? '$' + amount.toFixed(2) : '';
  }

  formatCurrencyOrig(amount: number) {
    return (amount) && amount > 0 ? '$' + amount.toFixed(2) : '';
  }

  clear() {
    location.reload();
  }
  
  print() {
    if (this.selectedCustomerList.length === 0 && (this.paymentTermCtrl.value === undefined || this.paymentTermCtrl.value === 0)) {
      this.alertService.printAllNotification().then(answer => {
        if (!answer.isConfirmed) { return; }
        this.executePrint();
      });
    }

    else this.executePrint();
  }

  executePrint() {
    this.alertService.showBlockUI("Fetching Print Data...");
    let statementDate = moment(new Date(this.form.value.statementDate)).format('MM/DD/YYYY');
    let paymentTermId = this.paymentTermCtrl.value !== undefined ? this.paymentTermCtrl.value : 0;
    let customerIds = (this.selectedCustomerList && this.selectedCustomerList.length > 0) ? this.selectedCustomerList.map(e => e.id) : [];

    this.reportService.getStatementReport(statementDate, paymentTermId, customerIds).subscribe(result => {
      this.alertService.hideBlockUI();

      if (result && result.length > 0) {
        this.statementReportList = result;
        this.cd.detectChanges();
        setTimeout(() => {
          window.print();
        }, 2000);
      }
      else {
        this.alertService.zeroRecordNotification("Outstanding Invoice");
      }
    });
  }

  checkValidEmail(row: CustomerDTO): boolean {
    let result = false;
    let contacts = row.contacts;
    
    if (contacts) {
      contacts.filter(c => c.isEmailStatement === true).forEach(e => {
        let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
        if (regexp.test(e.email.toLowerCase())) {
          result = true;
        }
      });
    }
    
    return result;
  }

  addCustomerNote(row: AgingBalanceReportDTO) {
    this.dialog.open(ToolbarNotificationsCustomerNoteDialog, {
      height: '60%',
      width: '60%',
      data: row
    }).afterClosed().subscribe(() => {
      this.cd.detectChanges();
    });
  }
}