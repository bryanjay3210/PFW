import { ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { CustomerCredit, CustomerDTO, Order, OrderDetail, Payment, PaymentDetail, PaymentTerm, PaymentType, PriceLevel, Role, User } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { LookupService } from 'src/services/lookup.service';
import { OrderService } from 'src/services/order.service';
import { MatTableDataSource } from '@angular/material/table';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { SelectionModel } from '@angular/cdk/collections';
import { CustomerService } from 'src/services/customer.service';
import { PaymentAddCreditComponent } from '../payment-add-credit/payment-add-credit.component';
import { CustomerCreditService } from 'src/services/customercredit.service';
import { PaymentCreditMemoComponent } from '../payment-credit-memo/payment-credit-memo.component';
import { PaymentCustomerListComponent } from '../payment-customer-list/payment-customer-list.component';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'vex-payment-create-update',
  templateUrl: './payment-create-update.component.html',
  styleUrls: ['./payment-create-update.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class PaymentCreateUpdateComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  columns: TableColumn<Order>[] = []
  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  inputType = 'password';

  dataSource: MatTableDataSource<any> | null;
  selection = new SelectionModel<Order>(true, []);

  roleList: Role[];
  userTypeList: Lookup[];
  accessTypeList: Lookup[];
  paymentTypeList: PaymentType[] = [];
  paymentTermList: PaymentTerm[] = [];
  priceLevelList: PriceLevel[] = [];
  customerInvoiceList: Order[] = [];
  paymentDetailsList: PaymentDetail[] = [];
  customerCreditList: Order[] = [];
  otherCustomerCreditList: Order[] = [];
  creditMemoList: Order[] = [];

  paymentTypeListFiltered: PaymentType[] = [];
 
  todayDate: Date = new Date();
  visible = false;
  isUseCredit: boolean = false;
  isCashOrCreditSelected: boolean = false;
  isCreditSelected: boolean = false;
 
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  currentCustomer: CustomerDTO = undefined;
  otherCustomer: CustomerDTO = undefined;
  creditBalance: number = 0;
  totalAmountCM: number = 0;
  selectedCustomers: CustomerDTO[] = [];
  selectedCustomerNames: string[] = [];
  selectedOtherCustomers: CustomerDTO[] = [];

  sortColumn: string = '';
  sortOrder: string = '';
  currentPaymentType: PaymentType;
  // pdsortColumn: string = '';
  // pdsortOrder: string = '';

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Payment,
    private dialogRef: MatDialogRef<PaymentCreateUpdateComponent>,
    private dialog: MatDialog,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private orderService: OrderService,
    private lookupService: LookupService,
    private customerService: CustomerService,
    private customerCreditService: CustomerCreditService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PaymentManagement);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();
    this.getLookUps();

    if (this.defaults) {
      this.columns.push({ label: 'Order Number', property: 'orderNumber', type: 'text', visible: true });
      this.columns.push({ label: 'Invoice Number', property: 'invoiceNumber', type: 'text', visible: true });
      this.columns.push({ label: 'Invoice Amount', property: 'invoiceAmount', type: 'number', visible: true });
      this.columns.push({ label: 'Payment Amount', property: 'paymentAmount', type: 'number', visible: true });
      this.columns.push({ label: 'Customer Credit Amount', property: 'customerCreditAmountUsed', type: 'number', visible: true });
      this.columns.push({ label: 'Invoice Balance', property: 'invoiceBalance', type: 'number', visible: true });
      this.columns.push({ label: 'Credit Memo Invoice Number', property: 'linkedInvoiceNumber', type: 'text', visible: true });

      this.getCustomerCredits(this.defaults.customerId);
      this.mode = 'update';
      this.isCashOrCreditSelected = this.defaults.paymentType === 'Cash' || this.defaults.paymentType === 'Credit Memo' || this.defaults.paymentType === 'Write Off';
      this.isCreditSelected = this.defaults.paymentType === 'Credit Memo';
      this.todayDate = moment(this.defaults.paymentDate).toDate();
      this.getCurrentCustomer(this.defaults.customerId);
      this.dataSource.data = this.defaults.paymentDetails;
    } else {
      this.columns.push({ label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true });
      this.columns.push({ label: 'Customer', property: 'customerName', type: 'text', visible: true });
      this.columns.push({ label: 'Order Number', property: 'orderNumber', type: 'text', visible: true });
      this.columns.push({ label: 'Invoice Number', property: 'invoiceNumber', type: 'text', visible: true });
      this.columns.push({ label: 'Order Date', property: 'createdDate', type: 'text', visible: true });
      this.columns.push({ label: 'Created By', property: 'createdBy', type: 'text', visible: true });
      this.columns.push({ label: 'Modified By', property: 'modifiedBy', type: 'text', visible: true });
      this.columns.push({ label: 'Status', property: 'orderStatusName', type: 'text', visible: true });
      this.columns.push({ label: 'Payment Status', property: 'paymentReference', type: 'text', visible: true });
      this.columns.push({ label: 'Total Amount', property: 'totalAmount', type: 'number', visible: true });
      this.columns.push({ label: 'Amount Paid', property: 'amountPaid', type: 'number', visible: true });
      this.columns.push({ label: 'Balance', property: 'balance', type: 'number', visible: true });
      this.defaults = {} as Payment;
    }

    this.form = this.fb.group({
      id: [PaymentCreateUpdateComponent.id++],
      accountNumber: [this.defaults.accountNumber || ''],
      totalAmountDue: [(this.defaults.totalAmountDue) ? this.defaults.totalAmountDue.toFixed(2) : '0.00' || '0.00'],
      paymentAmount: [(this.defaults.paymentAmount) ? this.defaults.paymentAmount.toFixed(2) : '0.00' || '0.00'],
      paymentBalance: [(this.defaults.paymentBalance) ? this.defaults.paymentBalance.toFixed(2) : '0.00' || '0.00'],
      paymentDate: [this.defaults.paymentDate || this.todayDate],
      paymentType: [this.defaults.paymentType || ''],
      referenceNumber: [this.defaults.referenceNumber || ''],
      customerCreditAmountUsed: [this.defaults.customerCreditId !== null ? this.defaults.customerCreditAmountUsed : '0.00' || '0.00'],
      creditMemoAmountUsed: [this.defaults.customerCreditAmountUsed || '0.00'],
      customerFilter: [''],
      customerName: [''],
      phoneNumber: [''],
      contactName: [''],
      paymentTermId: [''],
      priceLevelId: [''],
      discount: ['0'],
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
  }

  getCurrentCustomer(customerId: number) {
    this.customerService.getCustomerById(customerId).subscribe(result => {
      if (result) {
        this.currentCustomer = result;
        this.updateCustomerDetails();
      }
    });
  }
  
  async getLookUps() {
    await this.lookupService.getPaymentTerms().subscribe((result: PaymentTerm[]) => {
      if (result) {
        this.paymentTermList = result;
      }
    });

    await this.lookupService.getPriceLevels().subscribe((result: PriceLevel[]) => {
      if (result) {
        this.priceLevelList = result;
      }
    });

    await this.lookupService.getPaymentTypes().subscribe((result: PaymentType[]) => {
      if (result) {
        this.paymentTypeList = result;
        this.paymentTypeListFiltered = JSON.parse(JSON.stringify(this.paymentTypeList)) as typeof this.paymentTypeList;
        if (this.defaults && this.defaults.paymentType) {
          this.currentPaymentType = this.paymentTypeList.find(e => e.name.trim().toLowerCase() === this.defaults.paymentType.trim().toLowerCase());
          this.form.value.paymentType = this.currentPaymentType.id;
        }
      }
    });

  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }
  
  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ? this.selection.clear() : this.dataSource.data.forEach(row => this.selection.select(row));
    this.getTotalAmountSelected();
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  selectInvoice(invoice: Order) {
    this.selection.toggle(invoice);
    this.getTotalAmountSelected();
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        if (Number(this.form.value.paymentAmount) === 0 && Number(this.form.value.customerCreditAmountUsed) === 0 && this.form.value.paymentType !== 'Credit Memo') {
          this.alertService.validationFailedNotification('Payment Amount Error', 'Payment amount must be greater than zero!');
          return;
        }

        if (this.form.value.paymentType === 'Credit Memo' && Number(this.form.value.creditMemoAmountUsed) === 0) {
          this.alertService.validationFailedNotification('Credit Memo Amount Error', 'Credit Memo amount must be greater than zero!');
          return;
        }

        if (this.form.value.paymentType !== 'Cash' && this.form.value.paymentType !== 'Credit Memo' && this.form.value.paymentType !== 'Write Off') {
          if (this.form.value.referenceNumber === '') {
            this.alertService.validationRequiredNotification('Reference Number is Required!');
          return;
          }
        }

        if (Number(this.form.value.paymentAmount) > Number(this.form.value.totalAmountDue) && this.form.value.paymentType !== 'CreditMemo') {
          this.alertService.createOverpaymentNotification().then(answer => {
            if (!answer.isConfirmed) return;
            this.createPayment();
          });
        }
        else if (Number(this.form.value.paymentAmount) < Number(this.form.value.totalAmountDue)) {
          this.alertService.createUnderpaymentNotification().then(answer => {
            if (!answer.isConfirmed) return;
            this.createPayment();
          });
        }
        else {
          this.alertService.createNotification("Payment").then(answer => {
            if (!answer.isConfirmed) return;
            this.createPayment();
          });
        }
      }
      else this.alertService.validationNotification("Payment");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("Payment").then(answer => {
          if (!answer.isConfirmed) return;
          this.updatePayment();
        });
      }
      else this.alertService.validationNotification("Payment");
    }
  }

  createPayment() {
    let paymentList: Payment[] = [];
    let currentBalance = 0;

    if (this.currentCustomer) {
      currentBalance = Number(this.form.value.paymentAmount) - Number(this.form.value.totalAmountDue);
      const payment = {} as Payment;
      this.mapFormValuesToPayment(payment);
      paymentList.push(payment);
      this.dialogRef.close({payment:paymentList, balance:currentBalance});
    }
    else {
      let orderList: Order[] = [];
      let payment: Payment;
      let customerId = 0;
      currentBalance = Number(this.form.value.paymentAmount);
      let selectedRecords = this.selection.selected.sort((a, b) => a.customerId - b.customerId);
      selectedRecords.forEach(e => {
        if (customerId !== e.customerId) {
          if (orderList.length > 0) {
            payment = {} as Payment;
            this.mapFormValuesToPaymentMulti(orderList, currentBalance, payment)
            paymentList.push(payment);
            currentBalance = currentBalance - orderList.map(a => a.totalAmount).reduce((a, b) => a + b, 0);
            orderList = [];
            this.paymentDetailsList = [];
          }
          customerId = e.customerId
        }
        orderList.push(e);
      });

      if (orderList.length > 0) {
        payment = {} as Payment;
        this.mapFormValuesToPaymentMulti(orderList, currentBalance, payment)
        paymentList.push(payment);
        currentBalance = currentBalance - orderList.map(a => a.totalAmount).reduce((a, b) => a + b, 0);
        orderList = [];
        this.paymentDetailsList = [];
      }
    }

    this.dialogRef.close({payment:paymentList, balance:currentBalance});
  }

  updatePayment() {
    const payment = {} as Payment;
    this.mapFormValuesToPayment(payment);
    this.dialogRef.close(payment);
  }

  mapFormValuesToPaymentMulti(orderList: Order[], currentBalance: number, payment: Payment) {
    payment.accountNumber = orderList[0].accountNumber;
    payment.customerId = orderList[0].customerId;
    payment.paymentDate = this.form.value.paymentDate === '' ? moment(new Date('0001-01-01T00:00:00Z')) : moment(new Date(this.form.value.paymentDate));
    payment.paymentType = this.form.value.paymentType;
    
    if (payment.paymentType !== 'Cash' && payment.paymentType !== 'Credit Memo' && this.form.value.paymentType !== 'Write Off') {
      payment.referenceNumber = this.form.value.referenceNumber;
    }

    payment.totalAmountDue = orderList.map(a => a.totalAmount).reduce((a, b) => a + b, 0);
    payment.paymentAmount = currentBalance >= payment.totalAmountDue ? payment.totalAmountDue : currentBalance;
    payment.paymentBalance = currentBalance >= payment.totalAmountDue ? 0 : payment.totalAmountDue - currentBalance;

    payment.isActive = true;
    payment.isDeleted = false;
    payment.createdBy = this.currentUser.userName;
    payment.createdDate = moment(new Date());

    let paymentAmt = currentBalance >= payment.totalAmountDue ? payment.totalAmountDue : currentBalance;
    let selectedRecords = orderList.sort((a, b) => a.invoiceNumber - b.invoiceNumber);
    selectedRecords.forEach(e => {
      // if (paymentAmt > 0) {
        const paymentDetail = {} as PaymentDetail;
        paymentDetail.createdBy = payment.createdBy;
        paymentDetail.createdDate = payment.createdDate;
        paymentDetail.invoiceAmount = e.balance;
        paymentDetail.invoiceNumber = e.invoiceNumber;
        paymentDetail.isActive = payment.isActive;
        paymentDetail.isDeleted = payment.isDeleted;
        paymentDetail.orderNumber = e.orderNumber;
  
        let invoiceAmt = e.balance;
  
        paymentDetail.paymentAmount = paymentAmt >= invoiceAmt ? invoiceAmt : paymentAmt;
        paymentDetail.invoiceBalance = invoiceAmt - paymentDetail.paymentAmount;
        paymentAmt = paymentAmt > invoiceAmt ? paymentAmt - invoiceAmt : 0;
  
        this.paymentDetailsList.push(paymentDetail);

        let applies = payment.appliesTo
        payment.appliesTo = (applies && applies.length > 0) ? applies + ',' + paymentDetail.orderNumber.toString() : paymentDetail.orderNumber.toString();
      // }
    });

    payment.creditMemos = this.creditMemoList;
    //payment.appliesTo = selectedRecords.map(e => e.orderNumber).join(',');
    payment.paymentDetails = this.paymentDetailsList;
  }

  mapFormValuesToPayment(payment: Payment) {
    payment.accountNumber = this.form.value.accountNumber;
    payment.customerId = this.currentCustomer.id;
    payment.paymentDate = this.form.value.paymentDate === '' ? moment(new Date('0001-01-01T00:00:00Z')) : moment(new Date(this.form.value.paymentDate));
    payment.paymentType = this.form.value.paymentType;
    
    if (payment.paymentType !== 'Cash' && payment.paymentType !== 'Credit Memo' && payment.paymentType !== 'Write Off') {
      payment.referenceNumber = this.form.value.referenceNumber;
    }

    payment.totalAmountDue = this.form.value.totalAmountDue;
    
    if (this.totalAmountCM > 0) {
      payment.paymentAmount = 0;

      if (payment.totalAmountDue > this.totalAmountCM) {
        payment.paymentBalance = payment.totalAmountDue - this.totalAmountCM;
        payment.customerCreditAmountUsed = this.totalAmountCM;
      }
      else {
        payment.paymentBalance = 0;
        payment.customerCreditAmountUsed = payment.totalAmountDue;
      }

      payment.linkedInvoiceNumber = this.creditMemoList.sort((a, b) => a.invoiceNumber - b.invoiceNumber).map(e => e.invoiceNumber).join(',');
    }
    else {
      payment.paymentAmount = this.form.value.paymentAmount;
      payment.paymentBalance = this.form.value.paymentBalance;

      if (Number(this.form.value.customerCreditAmountUsed) > 0) {
        payment.customerCreditId = this.currentCustomer.customerCredit.id;
        payment.customerCreditAmountUsed = this.form.value.customerCreditAmountUsed;
      }
    }

    payment.isActive = true;
    payment.isDeleted = false;
    payment.createdBy = this.currentUser.userName;
    payment.createdDate = moment(new Date());

    let paymentAmt = this.form.value.paymentAmount;
    let customerCreditAmt = this.form.value.customerCreditAmountUsed;
    
    let selectedRecords = this.selection.selected.sort((a, b) => a.invoiceNumber - b.invoiceNumber);
    selectedRecords.forEach(e => {
      // if (paymentAmt > 0) {
        const paymentDetail = {} as PaymentDetail;
        paymentDetail.createdBy = payment.createdBy;
        paymentDetail.createdDate = payment.createdDate;
        paymentDetail.invoiceAmount = e.balance;
        paymentDetail.invoiceNumber = e.invoiceNumber;
        paymentDetail.isActive = payment.isActive;
        paymentDetail.isDeleted = payment.isDeleted;
        paymentDetail.orderNumber = e.orderNumber;

        let invoiceAmt = e.balance;

        if (this.totalAmountCM > 0) {
          let creditMemos = this.creditMemoList.sort((a, b) => a.invoiceNumber - b.invoiceNumber).filter(e => (e.balance * -1) > 0);
          for (let cm of creditMemos) {
            if (invoiceAmt === 0) break;

            let creditMemoAmt = cm.balance * -1; // Convert to positive amount...
            paymentDetail.customerCreditAmountUsed = paymentDetail.customerCreditAmountUsed === undefined ? 0 : paymentDetail.customerCreditAmountUsed;

            if (invoiceAmt >= creditMemoAmt) {
              paymentDetail.customerCreditAmountUsed += creditMemoAmt;
              paymentDetail.paymentAmount = 0;
              paymentDetail.invoiceBalance = invoiceAmt - creditMemoAmt;

              // Update Credit Memo Record
              cm.amountPaid += creditMemoAmt * -1;
              cm.balance = 0;
              cm.payment = cm.amountPaid;

              invoiceAmt -= creditMemoAmt;
              this.totalAmountCM -= creditMemoAmt;
              creditMemoAmt = 0;
            }
            else {
              paymentDetail.customerCreditAmountUsed += invoiceAmt;
              paymentDetail.paymentAmount = 0;
              paymentDetail.invoiceBalance = 0;

              // Update Credit Memo Record
              cm.amountPaid += invoiceAmt * -1;
              cm.balance -= invoiceAmt * -1;
              cm.payment = cm.amountPaid;

              creditMemoAmt -= invoiceAmt;
              this.totalAmountCM -= invoiceAmt;
              invoiceAmt = 0;
            }

            // Save Credit Memo Invoice Number used for the Payment Detail
            paymentDetail.linkedInvoiceNumber = (paymentDetail.linkedInvoiceNumber && paymentDetail.linkedInvoiceNumber.length > 0) ? paymentDetail.linkedInvoiceNumber + ',' + cm.invoiceNumber.toString() : cm.invoiceNumber.toString();

            // Save Order Invoice Number where the Credit Memo was applied.
            cm.linkedInvoiceNumber = (cm.linkedInvoiceNumber && cm.linkedInvoiceNumber.length > 0) ? cm.linkedInvoiceNumber + ',' + e.invoiceNumber.toString() : e.invoiceNumber.toString();
          }
        }
        else {
          if (customerCreditAmt > 0) {
            if (invoiceAmt > customerCreditAmt) {
              paymentDetail.customerCreditAmountUsed = customerCreditAmt;
              paymentDetail.paymentAmount = paymentAmt >= (invoiceAmt - customerCreditAmt) ? (invoiceAmt - customerCreditAmt) : paymentAmt;
              paymentDetail.invoiceBalance = (invoiceAmt - customerCreditAmt) - paymentDetail.paymentAmount;
              paymentAmt = paymentAmt > (invoiceAmt - customerCreditAmt) ? paymentAmt - (invoiceAmt - customerCreditAmt) : 0;
              customerCreditAmt = 0;
            }
            else { // invoceAmt <= customerCreditAmt
              paymentDetail.customerCreditAmountUsed = invoiceAmt;
              paymentDetail.paymentAmount = 0;
              paymentDetail.invoiceBalance = 0;
              customerCreditAmt = customerCreditAmt - invoiceAmt;
            }
          }
          else {
            paymentDetail.paymentAmount = paymentAmt >= (invoiceAmt) ? (invoiceAmt) : paymentAmt;
            paymentDetail.invoiceBalance = invoiceAmt - paymentDetail.paymentAmount;
            paymentAmt = paymentAmt > (invoiceAmt) ? paymentAmt - (invoiceAmt) : 0;
          }
        }

        this.paymentDetailsList.push(paymentDetail);

        let applies = payment.appliesTo
        payment.appliesTo = (applies && applies.length > 0) ? applies + ',' + paymentDetail.orderNumber.toString() : paymentDetail.orderNumber.toString();
      // }
    });

    payment.creditMemos = this.creditMemoList;
    //payment.appliesTo = selectedRecords.map(e => e.orderNumber).join(',');
    payment.paymentDetails = this.paymentDetailsList;
  }

  applyCreditMemoInPayment() {
    if (this.customerInvoiceList.length === 0) return;

    this.totalAmountCM = 0;
    this.creditMemoList.forEach(a => this.totalAmountCM += (a.balance * -1));
    
    this.form.get('customerCreditAmountUsed').setValue('0.00');
    this.form.get('creditMemoAmountUsed').setValue(this.totalAmountCM.toFixed(2));
    this.form.get('paymentType').setValue('Credit Memo');
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  openCustomerList() {
    this.dialog.open(PaymentCustomerListComponent, {
      height: '80%',
      width: '80%',
      data: { customerFilter: this.form.value.customerFilter }
    }).afterClosed().subscribe((customers: CustomerDTO[]) => {
      if (customers) {
        this.clearCustomerDetails();
        if (customers.length === 1) {
          this.currentCustomer = customers[0];
          this.getCustomerCredits(this.currentCustomer.id);
          this.paymentTypeListFiltered = JSON.parse(JSON.stringify(this.paymentTypeList)) as typeof this.paymentTypeList;
        }
        else {
          this.selectedCustomers = customers;
          this.selectedCustomerNames = customers.map(e =>e.customerName);
          this.paymentTypeListFiltered.splice(4,1);
        }
        
        this.getCustomersInvoices(customers.map(e => e.id));
        this.cd.detectChanges();
      }
    });
  }

  updateCustomerDetails() {
    this.form.get('customerName').setValue(this.currentCustomer.customerName);
    this.form.get('accountNumber').setValue(this.currentCustomer.accountNumber);
    this.form.get('phoneNumber').setValue(this.currentCustomer.phoneNumber);
    this.form.get('contactName').setValue(this.currentCustomer.contactName);
    this.form.get('discount').setValue(this.currentCustomer.discount);
    this.form.get('paymentTermId').setValue(this.currentCustomer.paymentTermId);
    this.form.get('priceLevelId').setValue(this.currentCustomer.priceLevelId);
    this.creditBalance = this.currentCustomer.customerCredit !== null ? this.currentCustomer.customerCredit.currentBalance : 0;
  }

  async getCustomerCredits(customerId: number) {
    await this.orderService.getCreditMemoByCustomerId(customerId).subscribe(result => {
      if (result) {
        this.customerCreditList = result;
        //this.getCustomerInvoices();
      }
    });
  }

  clearCustomerDetails() {
    this.form.get('customerName').setValue('');
    this.form.get('accountNumber').setValue('');
    this.form.get('phoneNumber').setValue('');
    this.form.get('contactName').setValue('');
    this.form.get('discount').setValue('');
    this.form.get('paymentTermId').setValue('');
    this.form.get('priceLevelId').setValue('');
    this.customerInvoiceList = [];
    this.dataSource.data = this.customerInvoiceList;
    this.creditBalance = 0;
    this.currentCustomer = undefined;
    this.otherCustomer = undefined;
    this.selectedCustomers = [];
    this.selectedCustomerNames = [];
  }

  getCustomerInvoices() {
    this.orderService.getInvoicesByCustomerId(this.currentCustomer.id).subscribe(result => {
      if (result && result.length > 0) {
        this.updateCustomerDetails();
        this.customerInvoiceList = result;
        this.dataSource.data = this.customerInvoiceList;
      }
      else {
        this.updateCustomerDetails();
        this.alertService.zeroRecordNotification("Customer Invoice");
      }
    });
  }

  async getCustomersInvoices(customerIds: number[]) {
    this.alertService.showBlockUI("Fetching Customer Invoice(s)...");
    await this.orderService.getInvoicesByCustomerIds(customerIds).subscribe(result => {
      if (result && result.length > 0) {
        this.alertService.hideBlockUI();
        if (this.currentCustomer) {
          this.updateCustomerDetails();
        }
        
        this.customerInvoiceList = result;
        this.dataSource.data = this.customerInvoiceList;
      }
      else {
        this.alertService.hideBlockUI();
        this.alertService.zeroRecordNotification("Customers Invoice");
      }
    });
  }

  getTotalAmountSelected() {
    if (this.isUpdateMode()) return;

    this.form.get('totalAmountDue').setValue('0.00');
    let totalDue: number = 0;
    this.selection.selected.forEach(e => {
      totalDue += e.balance;
    });

    totalDue = totalDue - this.form.value.customerCreditAmountUsed;

    let paymentAmount = this.form.value.paymentAmount;
    this.form.get('totalAmountDue').setValue(totalDue.toFixed(2));
    this.form.get('paymentAmount').setValue((this.form.value.paymentAmount) ? Number(this.form.value.paymentAmount).toFixed(2) : '0.00');
    this.form.get('paymentBalance').setValue((totalDue - (paymentAmount ? paymentAmount : 0)).toFixed(2));
  }

  validateCustomerCreditAmount() {
    if (Number(this.form.value.customerCreditAmountUsed) > this.currentCustomer.customerCredit.currentBalance) {
      this.alertService.validationFailedNotification('Customer Credit Error', 'Amount cannot be greater than the Credit Balance!');
      return;
    }

    this.getTotalAmountSelected();
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  formatPaymentAmount(amount: any) {
    if (amount && amount.length > 0) {
      let amt = amount.replaceAll('.', '');
      
      if (amt.length === 1) {
        amt = '0' + amt;
      }

      let position = amt.length - 2;
      amt = amt.substring(0, position) + '.' + amt.substring(position);
      this.form.get('paymentAmount').setValue((amt) ? Number(amt).toFixed(2) : '0.00');
    }
    else {
      this.form.get('paymentAmount').setValue('0.00');
    }

    this.getTotalAmountSelected();
  }

  paymentTypeValueChange(event: any) {
    this.isCashOrCreditSelected = (event === 'Cash' || event === 'Credit Memo' || event === 'Write Off');
    this.isCreditSelected = (event === 'Credit Memo');
  }

  addCustomerCredit() {
    if (this.currentCustomer === null || this.currentCustomer === undefined) return;
    this.dialog.open(PaymentAddCreditComponent, {
    }).afterClosed().subscribe((customerCredit: CustomerCredit) => {
      if (customerCredit) {
        // Process missing data
        customerCredit.customerId = this.currentCustomer.id;
        customerCredit.currentBalance = this.currentCustomer.customerCredit ? this.currentCustomer.customerCredit.currentBalance + customerCredit.amountPosted : customerCredit.amountPosted;
        customerCredit.previousBalance = this.currentCustomer.customerCredit ? this.currentCustomer.customerCredit.currentBalance : 0;
        customerCredit.previousRecordId = this.currentCustomer.customerCredit ? this.currentCustomer.customerCredit.id : 0;

        this.customerCreditService.createCustomerCredit(customerCredit).subscribe((result: CustomerCredit) => {
          if (result) {
            this.getCurrentCustomer(customerCredit.customerId);
            this.alertService.successNotification("Customer Credit", "Create");
          }
          else {
            this.alertService.failNotification("Customer Credit", "Create");
          }
        });
      }
    });
  }

  viewCredit() {
    this.dialog.open(PaymentCreditMemoComponent, {
      height: '70%',
      width: '70%',
      data: {customer: this.currentCustomer, credits: this.customerCreditList }
    }).afterClosed().subscribe((creditMemoList: Order[]) => {
      if (creditMemoList) {
        this.creditMemoList = creditMemoList;
        this.applyCreditMemoInPayment();
        this.getCustomerCredits(this.currentCustomer.id);
      }
      else {
        this.creditMemoList = [];
        this.totalAmountCM = 0;
        this.form.get('customerCreditAmountUsed').setValue('0.00');
        this.form.get('creditMemoAmountUsed').setValue('0.00');
        this.form.get('paymentType').setValue('');
        this.isCashOrCreditSelected = false;
        this.isCreditSelected = false;
        this.getCustomerCredits(this.currentCustomer.id);
      }
    });
  }

  otherCredit() {
    this.dialog.open(PaymentCustomerListComponent, {
      height: '80%',
      width: '80%',
      data: { customerFilter: undefined }
    }).afterClosed().subscribe((customers: CustomerDTO[]) => {
      if (customers) {
        this.getOtherCustomerCredits(customers);
        this.cd.detectChanges();
      }
    });
  }

  getOtherCustomerCredits(customers: CustomerDTO[]) {
    this.orderService.getCreditMemoByCustomerIds(customers.map(e => e.id)).subscribe(result => {
      if (result) {
        this.otherCustomerCreditList = result;
        this.dialog.open(PaymentCreditMemoComponent, {
          height: '70%',
          width: '70%',
          data: { customerNames: customers.map(e => e.customerName), credits: this.otherCustomerCreditList }
        }).afterClosed().subscribe((creditMemoList: Order[]) => {
          if (creditMemoList) {
            this.creditMemoList = creditMemoList;
            this.applyCreditMemoInPayment();
            this.getCustomerCredits(this.currentCustomer.id);
          }
          else {
            this.creditMemoList = [];
            this.totalAmountCM = 0;
            this.form.get('customerCreditAmountUsed').setValue('0.00');
            this.form.get('creditMemoAmountUsed').setValue('0.00');
            this.form.get('paymentType').setValue('');
            this.isCashOrCreditSelected = false;
            this.isCreditSelected = false;
            this.getCustomerCredits(this.currentCustomer.id);
          }
        });
      }
    });
  }
}
