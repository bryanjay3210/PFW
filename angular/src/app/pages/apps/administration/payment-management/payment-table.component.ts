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
import { PaymentService } from 'src/services/payment.service';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { PaymentPaginatedListDTO, Payment, User, TotalPaymentDTO, OverpaymentParameterDTO } from 'src/services/interfaces/models';
import { PaymentCreateUpdateComponent } from './payment-create-update/payment-create-update.component';
import moment from 'moment';
import { error } from 'console';
import { OrderService } from 'src/services/order.service';
import { delay } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'vex-payment-table',
  templateUrl: './payment-table.component.html',
  styleUrls: ['./payment-table.component.scss'],
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
export class PaymentTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<Payment>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Payment Id', property: 'id', type: 'text', visible: true },
    { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
    { label: 'Customer Id', property: 'customerId', type: 'text', visible: true },
    { label: 'Account Number', property: 'accountNumber', type: 'text', visible: true },
    { label: 'Payment Date', property: 'paymentDate', type: 'text', visible: true },
    { label: 'Payment Type', property: 'paymentType', type: 'text', visible: true },
    { label: 'Reference Number', property: 'referenceNumber', type: 'text', visible: true },
    { label: 'Total Amount Due', property: 'totalAmountDue', type: 'number', visible: true },
    { label: 'Payment Amount', property: 'paymentAmount', type: 'number', visible: true },
    { label: 'Customer Credit Amount', property: 'customerCreditAmountUsed', type: 'number', visible: true },
    { label: 'Payment Balance', property: 'paymentBalance', type: 'number', visible: true },
    { label: 'Applies To', property: 'appliesTo', type: 'text', visible: true },
    { label: 'Credit Memo Invoice Number', property: 'linkedInvoiceNumber', type: 'text', visible: true },
    
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');
  pageSize: number = 10;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';
  dataSource: MatTableDataSource<Payment> | null;
  selection = new SelectionModel<Payment>(true, []);
  searchCtrl = new UntypedFormControl();
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  isShowInactive: boolean = true;

  totalPaymentSummaryCA: TotalPaymentDTO;
  totalPaymentSummaryNV: TotalPaymentDTO;
  lastPayment: Payment;

  fromDateCtrl = new UntypedFormControl();
  toDateCtrl = new UntypedFormControl();

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private orderService: OrderService,
    private paymentService: PaymentService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PaymentManagement);
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
    this.fromDateCtrl.setValue('');
    this.toDateCtrl.setValue('');
    
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
    this.getPaginatedPayments();
  }

  getPaginatedPayments() {
    this.alertService.showBlockUI('Loading Payments...');
    if (!!this.search) this.search = this.search.trim();
    this.paymentService.getPaymentsPaginated(this.isShowInactive, this.pageSize, this.pageIndex, "PartNumber", "ASC", this.search).subscribe({
      next: (result) => {
        if (result) {
          this.dataSource.data = result.data;
          this.dataCount = result.recordCount;
          this.alertService.hideBlockUI();
          this.cd.detectChanges();
        }
        else {
          this.alertService.hideBlockUI();
          this.alertService.failNotification("Payments", "Fetch");
        }
      },
      error: (e) => {
        this.alertService.hideBlockUI();
        // NJPR 05032024 Temp Comment - this.alertService.errorNotification('An error was encountered while getting the Payments.');
        console.error(e)
      },
      complete: () => {
        console.info('complete');
        this.getDailyPaymentSummary();
      }
    });
  }

  getPaginatedPaymentsByDate() {
    this.alertService.showBlockUI('Loading Payments...');

    // let frDate = new Date(this.fromDateCtrl.value).toLocaleDateString();
    // let toDate = new Date(this.toDateCtrl.value).toLocaleDateString();
    let frDate = moment(new Date(this.fromDateCtrl.value)).toISOString();
    let toDate = moment(new Date(this.toDateCtrl.value)).toISOString();

    this.paymentService.getPaymentsByDatePaginated(this.pageSize, this.pageIndex, frDate, toDate).subscribe({
      next: (result) => {
        if (result) {
          this.dataSource.data = result.data;
          this.dataCount = result.recordCount;
          this.alertService.hideBlockUI();
          this.cd.detectChanges();
        }
        else {
          this.alertService.hideBlockUI();
          this.alertService.failNotification("Payments", "Fetch");
        }
      },
      error: (e) => {
        this.alertService.hideBlockUI();
        // NJPR 05032024 Temp Comment - this.alertService.errorNotification('An error was encountered while getting the Payments.');
        console.error(e)
      },
      complete: () => {
        console.info('complete');
        this.getPaymentSummaryByDate();
      }
    });
  }

  getDailyPaymentSummary() {
    this.totalPaymentSummaryCA = undefined;
    this.totalPaymentSummaryNV = undefined;
    let rawDate = new Date().setHours(0,0,0,0);
    let currentDate = new Date(rawDate).toISOString();

    this.paymentService.getDailyPaymentSummary(currentDate).subscribe(result => {
      if (result) {
        this.totalPaymentSummaryCA = result.caSummary;
        this.totalPaymentSummaryNV = result.nvSummary;
        this.cd.detectChanges();
      }
    });
  }

  getPaymentSummaryByDate() {
    this.totalPaymentSummaryCA = undefined;
    this.totalPaymentSummaryNV = undefined;

    // let frDate = new Date(this.fromDateCtrl.value).toLocaleDateString();
    // let toDate = new Date(this.toDateCtrl.value).toLocaleDateString();
    let frDate = moment(new Date(this.fromDateCtrl.value)).toISOString();
    let toDate = moment(new Date(this.toDateCtrl.value)).toISOString();

    this.paymentService.getPaymentSummaryByDate(frDate, toDate).subscribe(result => {
      if (result) {
        this.totalPaymentSummaryCA = result.caSummary;
        this.totalPaymentSummaryNV = result.nvSummary;
        this.cd.detectChanges();
      }
    });
  }

  async createPayment() {
    this.dialog.open(PaymentCreateUpdateComponent, {
      height: '100%',
      width: '100%',
    }).afterClosed().subscribe(async (result: any) => {
      if (result) {
        let payments: Payment[] = result.payment;
        let balance: number = result.balance;
        if (payments && payments.length > 0) {
          await this.processPayments(payments, balance);
          this.alertService.successNotification("Payment(s)", "Create");
          this.getPaginatedPayments();
          this.cd.detectChanges();
        }
      }

    });
  }

  async processPayments(payments: Payment[], balance: number) {
    for (let i = 0; i < payments.length; i++) {
      let amt = (i !== payments.length-1) ? 0 : balance;
      await this.createPaymentFunc(payments[i], amt);
    }
  }

  async createPaymentFunc(payment: Payment, balance: number) {
    await this.paymentService.createPayment(payment).subscribe({
      next: (result) => {
        if (result) {
          if (balance > 0) {
            this.lastPayment = result;
            this.createOverpayment(result, balance);
          }
        }
      },
      error: (e) => console.error(e),
      complete: () => console.info('complete') 
    });
  }

  async createOverpayment(payment: Payment, amount: number) {
    const param = {} as OverpaymentParameterDTO;
    param.customerId = payment.customerId;
    param.amount = amount;
    param.userName = this.currentUser.userName;
    param.notes = 'Over payment from Payment Id: ' + payment.id + ' (Payment Type: ' + payment.paymentType; 
    param.notes += ((this.lastPayment.paymentType !== 'Cash') ? (' Reference Number: ' + this.lastPayment.referenceNumber) : '') + ')';
    await this.orderService.createOverpayment(param).subscribe({
      next: (result) => {
        if (result) {
          this.alertService.failNotification("Overpayment with amount: $" + this.formatCurrency(amount), "Create");
        }
      },
      error: (e) => {
        this.alertService.failNotification("Overpayment with amount: $" + this.formatCurrency(amount), "Create");
        console.error(e)
      },
      complete: () => {
        console.info('complete') 
        //this.getPaginatedPayments();
        //this.cd.detectChanges();
      }
    })
  }

  updatePayment(payment: Payment) {
    this.dialog.open(PaymentCreateUpdateComponent, {
      height: '100%',
      width: '100%',
      data: payment
    }).afterClosed().subscribe(updatedPayment => {
      if (updatedPayment) {
        this.paymentService.updatePayment(updatedPayment).subscribe((result: boolean) => {
          if (result) {
            this.alertService.successNotification("Payment", "Update");
            this.getPaginatedPayments();
            this.cd.detectChanges();
          }
          else this.alertService.failNotification("Payment", "Update");
        });
      }
    });
  }

  deletePayment(payment: Payment) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (payment) {
      // this.userService.deletePayment([user]).subscribe((result: Payment[]) => (this.subject$.next(result)));
    }

    // this.inventories.splice(this.inventories.findIndex((existingPayment) => existingPayment.id === user.id), 1);
    // this.selection.deselect(user);
    // this.subject$.next(this.inventories);
  }

  deletePayments(inventories: Payment[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deletePayment(inventories).subscribe((result: Payment[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deletePayment(c));
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
      this.getPaginatedPayments();
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
    if (this.fromDateCtrl.value !== '' && this.toDateCtrl.value !== '') {
      this.getPaginatedPaymentsByDate();
    }
    else {
      this.getPaginatedPayments();
    }
    
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
  }

  searchPayments() {
    this.getPaginatedPayments();
  }

  showInactivePayments() {
    this.getData();
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  onDateChange() {
    this.searchCtrl.setValue('');
    this.toDateCtrl.setValue('');
  }

  clearDateSearch() {
    this.searchCtrl.setValue('');
    this.fromDateCtrl.setValue('');
    this.toDateCtrl.setValue('');
    this.getPaginatedPayments();
  }

  searchPaymentsByDate() {
    this.getPaginatedPaymentsByDate();
  }
}
