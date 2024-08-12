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
import { AgingBalanceReportDTO, Contact, CustomerDTO, PaymentTerm, StatementEmailParamDTO, StatementReportDTO, User } from 'src/services/interfaces/models';
import moment from 'moment';
import { MatDialog } from '@angular/material/dialog';
import { LookupService } from 'src/services/lookup.service';
import { ReportCustomerListComponent } from '../report-customer-list/report-customer-list.component';
import { ReportService } from 'src/services/report.service';
import { EmailService } from 'src/services/email.service';

import { HttpClient, HttpErrorResponse, HttpEventType } from '@angular/common/http';

import * as pdfMake from "pdfMake/build/pdfMake";
import * as pdfFonts from "pdfMake/build/vfs_fonts";
import jsPDF from 'jspdf';
import { ToolbarNotificationsCustomerNoteDialog } from 'src/@vex/layout/toolbar/toolbar-notifications/toolbar-notifications-customer-note-dialog/toolbar-notifications-customer-note-dialog';
const htmlToPdfmake = require("html-to-pdfmake");
(pdfMake as any).vfs = pdfFonts.pdfMake.vfs;

@UntilDestroy()
@Component({
  selector: 'vex-aging-balance-report',
  templateUrl: './aging-balance-report.component.html',
  styleUrls: ['./aging-balance-report.component.scss'],
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
export class AgingBalanceReportComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('location', { static: false }) location: ElementRef;
  @ViewChild('product', { static: false }) product: ElementRef;

  @Input()
  columns: TableColumn<AgingBalanceReportDTO>[] = [
    { label: 'Account Name', property: 'customerName', type: 'text', visible: true},
    { label: 'Account Number', property: 'accountNumber', type: 'number', visible: true },
    { label: 'Payment Term', property: 'paymentTermName', type: 'text', visible: true },
    { label: 'State', property: 'state', type: 'text', visible: true },
    { label: 'Phone Number', property: 'phoneNumber', type: 'text', visible: true },
    { label: 'Current 30 Days', property: 'current30Days', type: 'number', visible: true },
    { label: 'Over 30 Days', property: 'over30Days', type: 'number', visible: true },
    { label: 'Over 60 Days', property: 'over60Days', type: 'number', visible: true },
    { label: 'Over 90 Days', property: 'over90Days', type: 'number', visible: true },
    { label: 'Total Owed', property: 'totalOwed', type: 'number', visible: true },
    { label: 'Last Payment Date', property: 'lastPaymentDate', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
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

  selectedCustomerList: CustomerDTO[] = [];
  paymentTermList: PaymentTerm[] = [];
  stateList: any = [{'id': 'CA', 'name': 'CA'}, {'id': 'NV', 'name': 'NV'}];
  customerList: CustomerDTO[] = [];
  statementReportList: StatementReportDTO[] = [];
  dataSource: MatTableDataSource<AgingBalanceReportDTO> | null;

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
  
  constructor(
    private router: Router,
    private dialog: MatDialog,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private http: HttpClient,
    private emailService: EmailService,
    private lookupService: LookupService,
    private reportService: ReportService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.StatementReport);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.customerFilterCtrl.setValue('');
    this.stateCtrl.setValue(0);
    this.paymentTermCtrl.setValue(0);
    this.dueDate.setDate(this.cutOffDate.getDate() + 5);

    this.dataSource = new MatTableDataSource();
    this.getData();
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

  onDateChange() {
    this.cutOffDate = this.form.value.statementDate;
    this.dueDate.setDate(this.cutOffDate.getDate() + 5);
    this.form.get('paymentDueDate').setValue(this.dueDate);
  }

  getReport() {
    let statementDate = moment(new Date(this.form.value.statementDate)).format('MM/DD/YYYY');
    this.reportService.getAgingBalanceReport(statementDate).subscribe(result => {
      if (result) {
        this.dataSource.data = result;
      }
    });
  }

  openCustomerList() {
    this.dialog.open(ReportCustomerListComponent, {
      height: '100%',
      width: '100%',
      data: { stateFilter: this.stateCtrl.value, paymentTermId: this.paymentTermCtrl.value, customerFilter: this.customerFilterCtrl.value }
    }).afterClosed().subscribe((customers: CustomerDTO[]) => {
      if (customers && customers.length > 0) {
        // Get Customer Statement Totals
        let statementDate = moment(new Date(this.form.value.statementDate)).format('MM/DD/YYYY');
        let paymentTermId = this.paymentTermCtrl.value !== undefined ? this.paymentTermCtrl.value : 0;
        let customerIds = customers.map(e => e.id);
        this.reportService.getStatementTotalReport(statementDate, paymentTermId, customerIds).subscribe(result => {
          if (result) {
            result.forEach(e => {
              let idx = customers.findIndex(c => c.id === e.customerId);
              if (idx !== -1) {
                customers[idx].statementAmount = e.totalDue;
              }
            });
          }
        });

        customers.forEach(e => {
          if (this.selectedCustomerList.findIndex(c => c.id === e.id) === -1) {
            this.selectedCustomerList.push(e);    
          }
        });

        this.dataSource.data = this.selectedCustomerList;
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

  formatDateOnly(orderDate: any) {
    if (!orderDate || orderDate == null) return '';
    return moment(orderDate).format('MM/DD/YYYY');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  deleteCustomer(customer: CustomerDTO) {
    this.alertService.removeNotification('Customer').then(answer => {
      if (!answer.isConfirmed) return;
      
      this.selectedCustomerList.splice(this.selectedCustomerList.findIndex((e) => e.id === customer.id), 1);
      this.dataSource.data = this.selectedCustomerList;
      this.cd.detectChanges();
    });
  }

  getPaymentTermName(id: number) {
    return this.paymentTermList.find(e => e.id === id).termName;
  }

  clear() {
    this.form.get('statementDate').setValue(this.todayDate);
    this.onDateChange();
    this.paymentTermCtrl.setValue(0);
    this.stateCtrl.setValue(0);
    this.customerFilterCtrl.setValue('');
    this.selectedCustomerList = [];
    this.dataSource.data = [];
    this.cd.detectChanges();
  }
  
  email() {
    this.alertService.warningConfirmationNotification('This will send Emails to the Customers with the attached SOA.').then(answer => {
      if (!answer.isConfirmed) return;

      let statementDate = moment(new Date(this.form.value.statementDate)).toISOString();
      let paymentTermId = this.paymentTermCtrl.value !== undefined ? this.paymentTermCtrl.value : 0;
      let customerIds = (this.selectedCustomerList && this.selectedCustomerList.length > 0) ? this.selectedCustomerList.map(e => e.id) : [];
      let paymentDueDate = moment(new Date(this.form.value.paymentDueDate)).toISOString(); //moment(new Date(this.form.value.paymentDueDate)).format('MM/DD/YYYY');
      const statementEmailParam = {} as StatementEmailParamDTO;
      statementEmailParam.customerIds = customerIds;
      statementEmailParam.dueDate = moment(new Date(paymentDueDate));
      statementEmailParam.paymentTermId = paymentTermId;
      statementEmailParam.reportDate = moment(new Date(statementDate));

      this.emailService.sendStatementEmails(statementEmailParam).subscribe(result => {});
      this.alertService.sendingEmailNotification();
    });
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
    }).afterClosed().subscribe(r => {
      this.cd.detectChanges();
    });
  }
}