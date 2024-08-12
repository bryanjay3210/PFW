import { AfterViewInit, ChangeDetectorRef, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { SelectionModel } from '@angular/cdk/collections';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { AlertService } from 'src/services/alert.service';
import { Role } from 'src/services/interfaces/role.model';
import { Customer } from 'src/services/interfaces/customer.model';
import { CustomerDTO, Location, OrderPaginatedListDTO, Payment, PaymentDetail } from 'src/services/interfaces/models';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { Order, User } from 'src/services/interfaces/models';
import { OrderService } from 'src/services/order.service';
import moment from 'moment';
import { CustomerService } from 'src/services/customer.service';
import { PaymentService } from 'src/services/payment.service';

@UntilDestroy()
@Component({
  selector: 'vex-payment-credit-memo',
  templateUrl: './payment-credit-memo.component.html',
  styleUrls: ['./payment-credit-memo.component.scss'],

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
  ],
})

export class PaymentCreditMemoComponent implements OnInit {
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  orderColumns: TableColumn<Order>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Order #', property: 'orderNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Invoice #', property: 'invoiceNumber', type: 'text', visible: false },
    { label: 'Order Date', property: 'orderDate', type: 'text', visible: true },
    { label: 'Status', property: 'orderStatusName', type: 'text', visible: true },
    { label: 'Total Amount', property: 'totalAmount', type: 'number', visible: true },
    { label: 'Amount Used', property: 'amountPaid', type: 'number', visible: true },
    { label: 'Balance', property: 'balance', type: 'number', visible: true },
    { label: 'Quote #', property: 'quoteNumber', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Is Quote', property: 'isQuote', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Customer', property: 'customerName', type: 'text', visible: true },
    { label: 'Account #', property: 'accountNumber', type: 'text', visible: true },
    { label: 'Phone #', property: 'phoneNumber', type: 'text', visible: true },
  ];

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');

  form: UntypedFormGroup;
  totalAmount: number = 0;

  pageSize: number = 10;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  search: string = '';
  
  isCashOrCreditSelected: boolean = true;
  refundSelected: boolean = false;

  dataSource: MatTableDataSource<Order> | null;
  selection = new SelectionModel<Order>(true, []);
  orderSearchCtrl = new UntypedFormControl();

  roleList: Role[];
  customerList: Customer[];
  locationList = {} as Location[]
  orderTypeList: Lookup[];
  paymentDetailsList: PaymentDetail[] = [];

  paymentTypeList = [
    { id: 1, termName: 'Cash' },
    { id: 2, termName: 'Check' },
    { id: 3, termName: 'Credit Card' },
  ]

  sortColumn: string = '';
  sortOrder: string = '';
  currentCustomer: CustomerDTO = undefined;
  customerNames: string = undefined;
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  orderId: number;
  data: any;
  
  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<PaymentCreditMemoComponent>,
    private router: Router,
    private fb: UntypedFormBuilder,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private customerService: CustomerService,
    private paymentService: PaymentService,
    private orderService: OrderService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  get visibleOrderColumns() {
    return this.orderColumns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();
    
    if (this.defaults.credits) {
      this.dataSource.data = this.defaults.credits;
    }

    if (this.defaults.customer) {
      this.currentCustomer = this.defaults.customer;
    }

    if (this.defaults.customerNames) {
      this.customerNames = this.defaults.customerNames;
    }

    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.orderSearchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onOrderFilterChange(value));

    this.initializeFormGroup();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  initializeFormGroup() {
    this.form = this.fb.group({
      id: [PaymentCreditMemoComponent.id++],
      paymentType: [''],
      referenceNumber: [''],
      totalAmountDue: ['0.00'],
      paymentAmount: ['0.00'],
      paymentBalance: ['0.00'],
      totalAmount: ['0.00'],
    });
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
  }
  paymentTypeValueChange(event: any) {
    this.isCashOrCreditSelected = (event === 'Cash' || event === 'Credit Memo');
  }

  getCurrentCustomer(customerId: number) {
    this.customerService.getCustomerById(customerId).subscribe(result => {
      if (result) {
        this.currentCustomer = result;
      }
    });
  }

  selectCreditMemo(row: Order) {
    this.selection.toggle(row);
    this.getTotalAmountSelected();
  }

  getTotalAmountSelected() {
    this.totalAmount = 0;
    this.selection.selected.forEach(e => {
      this.totalAmount += (e.totalAmount - e.amountPaid); // Replace with Amount Used
    });

    this.totalAmount = this.totalAmount !== 0 ? this.totalAmount *= -1 : 0;

    this.form.get('totalAmountDue').setValue('0.00');
    let totalDue: number = 0;
    this.selection.selected.forEach(e => {
      totalDue += e.balance;
    });

    totalDue = totalDue * -1;
    let paymentAmount = totalDue; //this.form.value.paymentAmount ? this.form.value.paymentAmount : 0;
    let paymentBalance = 0; totalDue - paymentAmount;

    this.form.get('totalAmountDue').setValue(totalDue.toFixed(2));
    this.form.get('paymentAmount').setValue(paymentAmount.toFixed(2));
    this.form.get('paymentBalance').setValue(paymentBalance.toFixed(2));
  }

  cancel() {
    if (this.refundSelected) {
      this.refundSelected = false;
      return;
    }

    this.dialogRef.close(undefined);
  }

  applyCreditMemo() {
    this.dialogRef.close(this.selection.selected);
  }

  refundCreditMemo() {
    this.refundSelected = true;
    this.getTotalAmountSelected();
  }

  saveRefund() {
    // Create Refund Payment Here
    if (this.form.valid) {
      this.alertService.createNotification("Refund").then(answer => {
        if (!answer.isConfirmed) return;
        this.createRefund();
      });
    }
    else this.alertService.validationNotification("Refund");

    this.getTotalAmountSelected();
  }

  createRefund() {
    const payment = {} as Payment;
    this.mapFormValuesToPayment(payment);

    // Save Payment Refund
    this.paymentService.createRefund(payment).subscribe((result: Payment) => {
      if (result) {
        this.alertService.successNotification("Refund", "Create");
        // Get Credit Memos
      }
      else {
        this.alertService.failNotification("Refund", "Create");
      }

      this.getCustomerCredits(this.currentCustomer.id);
    });
    
    this.refundSelected = false;
  }

  mapFormValuesToPayment(payment: Payment) {
    payment.accountNumber = this.currentCustomer.accountNumber;
    payment.customerId = this.currentCustomer.id;
    payment.paymentDate = moment(new Date());
    payment.paymentType = this.form.value.paymentType;
    
    if (payment.paymentType !== 'Cash') {
      payment.referenceNumber = this.form.value.referenceNumber;
    }

    payment.totalAmountDue = this.form.value.totalAmountDue * -1;
    payment.paymentAmount = this.form.value.paymentAmount * -1;
    payment.paymentBalance = 0;
    payment.isActive = true;
    payment.isDeleted = false;
    payment.createdBy = this.currentUser.userName;
    payment.createdDate = moment(new Date());
    
    let selectedRecords = this.selection.selected.sort((a, b) => a.invoiceNumber - b.invoiceNumber);
    selectedRecords.forEach(e => {
      const paymentDetail = {} as PaymentDetail;
      paymentDetail.createdBy = payment.createdBy;
      paymentDetail.createdDate = payment.createdDate;
      paymentDetail.invoiceNumber = e.invoiceNumber;
      paymentDetail.isActive = payment.isActive;
      paymentDetail.isDeleted = payment.isDeleted;
      paymentDetail.orderNumber = e.orderNumber;
      
      paymentDetail.invoiceAmount = e.balance;
      paymentDetail.paymentAmount = e.balance;
      paymentDetail.invoiceBalance = 0;
      
      this.paymentDetailsList.push(paymentDetail);
    });

    payment.creditMemos = selectedRecords;
    payment.linkedInvoiceNumber = selectedRecords.sort((a, b) => a.invoiceNumber - b.invoiceNumber).map(e => e.invoiceNumber).join(',');
    payment.appliesTo = selectedRecords.sort((a, b) => a.orderNumber - b.orderNumber).map(e => e.orderNumber).join(',');
    payment.paymentDetails = this.paymentDetailsList;
  }

  getCustomerCredits(customerId: number) {
    this.orderService.getCreditMemoByCustomerId(customerId).subscribe(result => {
      if (result) {
        this.dataSource.data = result;
      }
    });
  }

  disableSaveRefund() {
    return Number(this.form.value.totalAmountDue) !== Number(this.form.value.paymentAmount);
  }

  disableApplyCredit() {
    return this.selection.selected.length === 0 || this.refundSelected;
  }

  getData() {
    this.getPaginatedOrdersList(undefined);
  }

  getPaginatedOrdersList(order: Order) {
    this.alertService.showBlockUI('Loading Orders...');
    if (!!this.search) this.search = this.search.trim();
    this.orderService.getOrdersPaginated(0, this.pageSize, this.pageIndex, "OrderNumber", "DESC", this.search).subscribe((result: OrderPaginatedListDTO) => {
      if (result) {
        this.dataSource.data = result.data;
        this.dataCount = result.recordCount;
        this.alertService.hideBlockUI();
      }
    });
  }

  onOrderFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.search = value;
    if (value.length == 0) {
      this.getPaginatedOrdersList(undefined);
    }
  }

  toggleOrderColumnVisibility(column, event) {
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
      this.getTotalAmountSelected();
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
    if (this.locationList) {
      let entity = this.locationList.find(e => e.id === value);
      return entity ? entity.locationName : '';
    }
    return '';
  }

  getBooleanText(value: boolean) {
    return value === false ? 'False' : 'True';
  }

  searchOrders() {
    this.getPaginatedOrdersList(undefined);
  }

  onOrderPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    //this.getPaginatedOrdersList(undefined);
  }

  convertIsQuote(IsQuote: boolean) {
    return IsQuote ? 'Quote' : 'Order';
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }
}