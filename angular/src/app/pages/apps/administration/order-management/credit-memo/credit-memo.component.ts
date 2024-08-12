import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, ValidationErrors } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AlertService } from 'src/services/alert.service';
import { CustomerService } from 'src/services/customer.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User, Order, CustomerDTO, ProductDTO, OrderDetail, Warehouse, ProductFilterDTO, PaymentHistoryDTO } from 'src/services/interfaces/models';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { Observable } from 'rxjs/internal/Observable';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import moment from 'moment';
import { LookupService } from 'src/services/lookup.service';
import { PaymentService } from 'src/services/payment.service';

@UntilDestroy()
@Component({
  selector: 'vex-credit-memo',
  templateUrl: './credit-memo.component.html',
  styleUrls: ['./credit-memo.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class CreditMemoComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  
  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";
  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  currentCustomer = {} as CustomerDTO;
  originalOrder = {} as Order;
  productFilterDTO = {} as ProductFilterDTO;
  customersList: CustomerDTO[];
  orderDetailsList: OrderDetail[] = [];
  warehouseList: Warehouse[] = [];
  paymentHistoryList: PaymentHistoryDTO[] = [];
  discountList: Order[] = [];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  vendorOverride = UserPermission.NoAccess;

  subject$: ReplaySubject<OrderDetail[]> = new ReplaySubject<OrderDetail[]>(1);
  data$: Observable<OrderDetail[]> = this.subject$.asObservable();
  dataSource: MatTableDataSource<OrderDetail> | null;
  selection = new SelectionModel<OrderDetail>(true, []);
  searchCtrl = new UntypedFormControl()

  selectedContact: string;
  summaryDiscount: number = 0;
  summarySubTotal: number = 0;
  summaryTaxRate: number = 0;
  summaryTax: number = 0;
  summaryTotal: number = 0;

  inputC: string = '';
  inputN: string = '';
  inputP: number = 0;
  inputQ: number = 1;

  todayDate: Date = new Date();
  showModal = true;
  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  isQuote: boolean = false;
  isCreditMemo: boolean = false;
  isDisplayReasonNotes: boolean = false;
  isDisplayDelivery: boolean = false;

  returnReasonList = 
  [
    { code:2, name: 'Agent Sold Wrong' }, // -  Required : enter why its wrong
    { code:3, name: 'Car Totaled' },
    { code:5, name: 'Customer Do Not Need' },
    { code:10, name: 'Customer Ordered Wrong' },
    { code:1, name:'Damaged' },
    { code:4, name: 'Defective' },
    { code:9, name: 'Got Somewhere Else' },
    { code:7, name: 'Manager Approval' }, // - Required : enter manager/supervisor name
    { code:6, name: 'Price and Billing Adjustment' },
    { code:8, name: 'Price Too High' },
  ]

  returnTypeList = 
  [
    { code:1, name:'Incoming' },
    { code:2, name: 'Outgoing' },
  ]

  columns: TableColumn<OrderDetail>[] = [
    { label: 'IMAGE', property: 'imageUrl', type: 'image', visible: true },
    { label: 'QTY', property: 'orderQuantity', type: 'number', visible: true, cssClasses: ['font-medium', 'input'] },
    { label: 'VndCode', property: 'vendors', type: 'button', visible: true, cssClasses: ['font-medium', 'input'] },
    { label: 'STK', property: 'onHandQuantity', type: 'number', visible: true },
    { label: 'PART#', property: 'partNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'YEAR', property: 'yearFrom', type: 'number', visible: true },
    { label: 'DESCRIPTION', property: 'partDescription', type: 'text', visible: true },
    { label: 'PLINKS', property: 'partsLinks', type: 'text', visible: true },
    { label: 'LIST PRC', property: 'listPrice', type: 'number', visible: true },
    { label: 'PRICE', property: 'wholesalePrice', type: 'number', visible: true },
    { label: 'DISC', property: 'discountedPrice', type: 'number', visible: true },
    { label: 'DISC(%)', property: 'discount', type: 'number', visible: true },
    { label: 'REC DISC(%)', property: 'discountRecord', type: 'number', visible: true },
    { label: 'RSTK FEE', property: 'restockingFee', type: 'number', visible: false },
    { label: 'RSTK AMT', property: 'restockingAmount', type: 'number', visible: true },
    { label: 'TOTAL', property: 'totalAmount', type: 'number', visible: true },
  ];
  
  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<CreditMemoComponent>,
    private fb: UntypedFormBuilder,
    private customerService: CustomerService,
    private lookupService: LookupService,
    private paymentService: PaymentService,
    private cd: ChangeDetectorRef,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();
    this.productFilterDTO.year = 0;
    this.productFilterDTO.categoryIds = [];
    this.productFilterDTO.sequenceIds = [];
    this.productFilterDTO.make = '';
    this.productFilterDTO.model = '';
    this.getLookups();

    if (this.defaults.dataSource) {
      this.mode = 'update';
      this.isCreditMemo = this.defaults.type === 'CM';
      this.discountList = this.defaults.discounts ? this.defaults.discounts : [];
      this.defaults = this.defaults.dataSource;
      this.isQuote = this.defaults.isQuote;
      this.todayDate = moment(this.defaults.deliveryDate).toDate();
      this.originalOrder = this.defaults;
      this.orderDetailsList = this.defaults.orderDetails;
      this.dataSource.data = this.orderDetailsList;
      this.summaryDiscount = this.defaults.discount;
      this.summaryTax = this.defaults.totalTax;
      this.summaryTaxRate = this.defaults.taxRate;
      this.summarySubTotal = this.defaults.subTotalAmount;
      this.summaryTotal = this.defaults.totalAmount;

      this.getCurrentCustomer();
      this.getPaymentHistory();
    } else {
      this.defaults = {} as Order;
    }

    //this.initializeFormGroup();
    
    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  getPaymentHistory() {
    this.paymentService.getPaymentHistoryByOrderNumber(this.defaults.orderNumber).subscribe(result => {
      if (result) {
        this.paymentHistoryList = result;
      }
    });
  }

  initializeFormGroup() {
    this.form = this.fb.group({
      id: [CreditMemoComponent.id++],
      orderNumber: [this.defaults.orderNumber || '0'],
      invoiceNumber: [this.defaults.invoiceNumber || ''],
      summaryDiscount: [this.defaults.discount || 0],
      summaryDiscountAmount: [this.getDiscountAmount() || 0.00],
      summarySubTotal: [this.defaults.subTotalAmount ? this.defaults.subTotalAmount.toFixed(2) : 0.00 || 0.00],
      summaryTax: [this.defaults.totalTax ? this.defaults.totalTax.toFixed(2) : '0.00' || '0.00'],
      summaryTaxRate: [this.defaults.taxRate || 0],
      summaryTotal: [this.defaults.totalAmount ? this.defaults.totalAmount.toFixed(2) : 0.00 || 0.00],
      restockingFee: ['0'],
      restockingAmount: ['0.00'],
      returnType: [this.defaults.rgaType, 0],
      returnReason: [this.defaults.rgaReason, 0],
      returnReasonNotes: [this.defaults.rgaReasonNotes, ''],
      deliveryDate: [this.defaults.deliveryDate, ''],
      deliveryRoute: [this.defaults.deliveryRoute, ''],
      warehouseTracking: [this.defaults.warehouseTracking, ''],
    });

    //this.form.get('summaryDiscountAmount').setValue(this.getDiscountAmount());
    // NJPR 
    this.handleSummaryOnChangeEvent2();
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  getDiscountAmount(): any {
    let discountAmount = this.defaults.subTotalAmount * (this.defaults.discount / 100);
    return discountAmount.toFixed(2);
  }

  getLookups() {
    this.lookupService.getWarehouses().subscribe((result: Warehouse[]) => {
      if (result) {
        this.warehouseList = result;
      }
    });
  }

  customFilter = function (countries: any[], query: string): any[] {
    return countries.filter(x => x.name.toLowerCase().contains(query.toLowerCase()));
  };
  
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
    this.dataSource.filter = value;
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(column: TableColumn<T>) {
    return column.property;
  }

  getCurrentCustomer(): void {
    this.customerService.getCustomerById(this.defaults.customerId).subscribe((result: CustomerDTO) => {
      if (result) {

        this.initializeFormGroup();
        this.currentCustomer = result;
        this.selectedContact = this.defaults.orderedBy;
        this.cd.detectChanges();
      }
    })
  }

  save() {
    if (this.form.valid) {
      if (this.isCreditMemo) {
        if (this.validOrderLineItems()) {
          this.alertService.createNotification('Credit Memo').then(answer => {
            if (!answer.isConfirmed) { return; }
            this.createCreditMemo();
          });
        }
      }
      else { //RGA
        if (this.validOrderLineItems()) {
          let message = "";
          this.dataSource.data.forEach(element => {
            message += 'Part Number: ' + element.partNumber + ' = ' + element.orderQuantity + '<br>';
          });
          message += '<br><br> Are you sure you want to create the RGA?';
        
          this.alertService.confirmRGAQuantities(message).then(answer => {
            if (!answer.isConfirmed) { return; }
            this.createCreditMemo();
            
          });
        }
      }
    }
    else {
      this.getFormValidationErrors();
      this.alertService.validationNotification("Order");
    }
  }

  getFormValidationErrors() {
    Object.keys(this.form.controls).forEach(key => {
      const controlErrors: ValidationErrors = this.form.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach(keyError => {
          console.log('Key control: ' + key + ', keyError: ' + keyError + ', err value: ', controlErrors[keyError]);
        });
      }
    });
  }

  // NJPR
  validOrderLineItems(): boolean {
    let result = true;
    let title = this.isCreditMemo ? 'Credit Memo' : 'RGA' + ' Validation';
    let message = '';

    // Validate Order Quantity
    for (const e of this.dataSource.data.filter(d => d.orderQuantity === 0)) {
      message = 'Order Part Number ' + e.partNumber + ' has Zero quantity, please change the quantity to at least 1.'
      this.alertService.validationFailedNotification(title, message);
      result = false;
      break;
    }

    // Validate Wholesale Price Equals Zero
    for (const e of this.dataSource.data.filter(d => d.wholesalePrice === 0)) {
      message = 'Order Part Number ' + e.partNumber + ' has Wholesale price equals to Zero, please change the price to at least $1.00.'
      this.alertService.validationFailedNotification(title, message);
      result = false;
      break;
    }

    // Validate List Price is LESS than the Wholesale Price 
    for (const e of this.dataSource.data.filter(d => d.wholesalePrice > d.listPrice)) {
      message = 'Order Part Number ' + e.partNumber + ' has Wholesale price greater than the List price, please lower the Wholesale price or increase the List price value.'
      this.alertService.validationFailedNotification(title, message);
      result = false;
      break;
    }

    if (this.isDisplayReasonNotes && (!this.form.value.returnReasonNotes || this.form.value.returnReasonNotes.length === 0)) {
      message = 'RGA Reason Note is required';
      this.alertService.validationFailedNotification(title, message);
      return false;
    }

    if (this.isDisplayDelivery && (this.form.value.deliveryDate === null || this.form.value.deliveryDate === '')) {
      message = 'Pickup Date is required';
      this.alertService.validationFailedNotification(title, message);
      return false;
    }

    if (this.isDisplayDelivery && (this.form.value.deliveryRoute === null || this.form.value.deliveryRoute === '' || this.form.value.deliveryRoute === 0)) {
      message = 'Pickup Route is required';
      this.alertService.validationFailedNotification(title, message);
      return false;
    }

    return result;
  }

  createCreditMemo() {
    const order = {} as Order;
    this.mapFormValuesToOrder(order);
    this.dialogRef.close(order);
  }

  mapFormValuesToOrder(order: Order) {
    // this.lookupService.getPriceLevels().subscribe((result: PriceLevel[]) => {
    //   if (result) {
    //     this.priceLevelList = result;
    //   }
    // });
    
    //order.quantity = this.defaults.quantity;
    order.accountNumber = this.defaults.accountNumber;
    order.customerId = this.defaults.customerId;
    order.customerName = this.defaults.customerName;
    order.phoneNumber = this.defaults.phoneNumber;
    order.priceLevelId = this.defaults.priceLevelId;
    order.priceLevelName = this.defaults.priceLevelName; // this.priceLevelList.find(e => e.id === order.priceLevelId).levelName;
    order.paymentTermId = this.defaults.paymentTermId;
    order.paymentTermName = this.defaults.paymentTermName; // this.paymentTermList.find(e => e.id === order.paymentTermId).termName;
    order.orderStatusId = this.isCreditMemo ? 5 : 9;
    order.orderStatusName = this.isCreditMemo ? 'Credit Memo' : 'RGA';
    order.warehouseId = this.currentCustomer.state === 'CA' ? 1 : 2;
    order.warehouseName = this.warehouseList.find(e => e.id === order.warehouseId).warehouseName;
    order.user = this.currentUser.userName;
    order.billAddress = this.defaults.billAddress;
    order.billCity = this.defaults.billCity;
    order.billContactName = this.defaults.billContactName;
    order.billPhoneNumber = this.defaults.billPhoneNumber;
    order.billState = this.defaults.billState;
    order.billZipCode = this.defaults.billZipCode;
    order.billZone = this.defaults.billZone;
    order.shipAddressName = this.defaults.shipAddressName;
    order.shipAddress = this.defaults.shipAddress;
    order.shipCity = this.defaults.shipCity;
    order.shipContactName = this.defaults.shipContactName;
    order.shipPhoneNumber = this.defaults.shipPhoneNumber;
    order.shipState = this.defaults.shipState;
    order.shipZipCode = this.defaults.shipZipCode;
    order.shipZone = this.defaults.shipZone;
    order.purchaseOrderNumber = this.defaults.purchaseOrderNumber;
    order.orderedBy = this.defaults.orderedBy;
    order.orderedByEmail = this.defaults.orderedByEmail;
    order.orderedByPhoneNumber = this.defaults.orderedByPhoneNumber;
    order.orderedByNotes = this.defaults.orderedByNotes;
    // order.deliveryType = this.form.value.returnType && this.form.value.returnType === 2 ? 2 : this.defaults.deliveryType;
    order.deliveryType = this.defaults.deliveryType;
    order.deliveryDate = this.form.value.returnType && this.form.value.returnType === 2 ? moment(new Date(this.form.value.deliveryDate)) : this.defaults.deliveryDate;
    order.deliveryRoute = this.form.value.returnType && this.form.value.returnType === 2 ? this.form.value.deliveryRoute : this.defaults.deliveryRoute;

    order.orderDate = moment(new Date()); //this.defaults.orderDate;
    order.isActive = this.defaults.isActive;
    order.isDeleted = this.defaults.isDeleted;
    order.isQuote = this.defaults.isQuote;
    order.createdBy = this.defaults.createdBy; 
    order.createdDate = order.orderDate;
    order.modifiedBy = this.currentUser.userName;
    order.modifiedDate = order.orderDate;
    order.originalInvoiceNumber = this.defaults.invoiceNumber;

    order.discount = this.form.value.summaryDiscount;
    order.taxRate = this.form.value.summaryTaxRate;
    order.totalTax = this.form.value.summaryTax;
    order.subTotalAmount = this.form.value.summarySubTotal;
    order.totalAmount = this.form.value.summaryTotal;
    order.restockingFee = this.form.value.restockingFee;
    order.restockingAmount = this.form.value.restockingAmount;
    order.amountPaid = 0;
    order.balance = order.totalAmount;

    if (!this.isCreditMemo) {
      order.rgaType = this.form.value.returnType ? this.form.value.returnType : 0;
      order.rgaReason = this.form.value.returnReason ? this.form.value.returnReason : 0;
      order.rgaReasonNotes = this.form.value.returnReasonNotes ? this.form.value.returnReasonNotes : '';
    }

    order.orderDetails = this.mapProductsToOrderDetails(order);
    order.currentCost = order.orderDetails.map(e => e.unitCost).reduce(function (a, b) { return a + b });
  }

  mapProductsToOrderDetails(order: Order): OrderDetail[] {
    const result: OrderDetail[] = [];
    this.dataSource.data.forEach(element => {
      const orderDetail = {} as OrderDetail;
      orderDetail.orderId = 0;
      orderDetail.productId = element.productId;
      orderDetail.orderQuantity = element.orderQuantity;
      orderDetail.location = element.location;
      orderDetail.partNumber = element.partNumber;
      orderDetail.partDescription = element.partDescription;
      orderDetail.brand = element.brand;
      orderDetail.mainPartsLinkNumber = element.mainPartsLinkNumber;
      orderDetail.mainOEMNumber = element.mainOEMNumber;

      orderDetail.vendorCode = element.vendorCode;
      orderDetail.vendorPartNumber = element.vendorPartNumber;
      orderDetail.vendorPrice = element.vendorPrice;
      orderDetail.vendorOnHand = element.vendorOnHand;
      orderDetail.onHandQuantity = element.onHandQuantity;
      orderDetail.yearFrom = element.yearFrom;
      orderDetail.yearTo = element.yearTo;

      orderDetail.listPrice = element.listPrice;
      orderDetail.wholesalePrice = element.wholesalePrice;
      orderDetail.price = element.price;
      orderDetail.discountedPrice = element.discountedPrice;
      orderDetail.totalAmount = element.totalAmount;
      orderDetail.restockingFee = element.restockingFee;
      orderDetail.restockingAmount = element.restockingAmount;

      orderDetail.partSize = element.partSize;
      orderDetail.categoryId = element.categoryId;

      orderDetail.partsLinks = element.partsLinks;
      orderDetail.oeMs = element.oeMs;
      orderDetail.vendorCodes = element.vendorCodes;

      orderDetail.isActive = this.defaults.isActive;
      orderDetail.isDeleted = this.defaults.isDeleted;
      orderDetail.createdBy = order.createdBy;
      orderDetail.createdDate = order.createdDate;
      orderDetail.modifiedBy = order.modifiedBy;
      orderDetail.modifiedDate = order.modifiedDate;
      
      orderDetail.unitCost = this.getUnitCostTotal(element);
      orderDetail.warehouseTracking = element.warehouseTracking;
      //orderDetail.statusId = 5;

      // if (this.isUpdateMode()) {
      //   orderDetail.stocks = element.stocks;
      //   orderDetail.statusId = element.statusId;
      //   orderDetail.modifiedBy = this.currentUser.userName;
      //   orderDetail.modifiedDate = moment(new Date());
      // }

      result.push(orderDetail);
    });

    return result;
  }

  getUnitCostTotal(element: OrderDetail): number {
    let result = 0;
    if (element.vendorCode.trim().length == 0) {
      result = element.orderQuantity * element.price;
    }
    else {
      if (element.onHandQuantity == 0) {
        result = element.orderQuantity * element.vendorPrice;
      }
      else {
        let vendorQty = element.orderQuantity - element.onHandQuantity;
        result = (element.onHandQuantity * element.price) + (vendorQty * element.vendorPrice);
      }
    }

    return result > 0 ? this.isCreditMemo ? result * -1 : result : 0;
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  setDefaultVendor(orderDetail: OrderDetail) {
    let defaultVendor = undefined;

    if (orderDetail.onHandQuantity === 0) {
      let vendorCatalogs = orderDetail.vendorCatalogs.filter(e => Number(e.onHand) > 0);
      defaultVendor = vendorCatalogs.find(e => e.price === Math.min.apply(null, vendorCatalogs.map(function (a) { return a.price; })));
    }

    orderDetail.vendorCode = defaultVendor ? defaultVendor.vendorCode : '';
    orderDetail.vendorPartNumber = defaultVendor ? defaultVendor.vendorPartNumber : '';
    orderDetail.vendorPrice = defaultVendor ? defaultVendor.price : 0;
    orderDetail.vendorOnHand = defaultVendor ? defaultVendor.onHand : 0;
  }

  getDiscountedPrice(product: ProductDTO): number {
    let originalPrice = 0
    switch (this.currentCustomer.priceLevelId) {
      case 1: {
        originalPrice = product.priceLevel1;
        break;
      }
      case 2: {
        originalPrice = product.priceLevel2;
        break;
      }
      case 3: {
        originalPrice = product.priceLevel3;
        break;
      }
      case 4: {
        originalPrice = product.priceLevel4;
        break;
      }
      case 5: {
        originalPrice = product.priceLevel5;
        break;
      }
      case 6: {
        originalPrice = product.priceLevel6;
        break;
      }
      case 7: {
        originalPrice = product.priceLevel7;
        break;
      }
      default: originalPrice = product.priceLevel8;
    }

    return originalPrice - (originalPrice / 100 * this.form.value.discount)
  }

  getWholesalePrice(product: ProductDTO): number {
    switch (this.currentCustomer.priceLevelId) {
      case 1: return product.priceLevel1;
      case 2: return product.priceLevel2;
      case 3: return product.priceLevel3;
      case 4: return product.priceLevel4;
      case 5: return product.priceLevel5;
      case 6: return product.priceLevel6;
      case 7: return product.priceLevel7;
      default: return product.priceLevel8;
    }
  }

  handleSummaryOnChangeEvent() {
    let restockingFee = this.form.value.restockingFee ? Number(this.form.value.restockingFee) : 0; // Fee is now percentage
    let summarySubTotal = 0;
    let summaryTotal = 0;
    let summaryRestockingAmount = 0;
    let orderDiscountAmount = 0;
    let recordDiscountAmount = 0;
    this.form.get('restockingAmount').setValue(0);

    // let discount = this.originalOrder.discount ? this.originalOrder.discount : 0;
    // let taxRate = this.originalOrder.taxRate ? this.originalOrder.taxRate : 0;
    // let taxAmount = 0; //this.originalOrder.totalTax ? this.originalOrder.totalTax : 0;

    this.orderDetailsList.forEach(e => {
      let restockingAmount = 0;
      //Reset Values before processing
      e.restockingFee = 0;
      e.restockingAmount = 0;
      e.totalAmount = 0;

      //Discount came from Order Detail
      let discount = e.discount;
      if (discount > 0) {
        orderDiscountAmount += (e.wholesalePrice - e.discountedPrice) * e.orderQuantity;
      }

      // Discount came from Record
      let discOrder = this.discountList.find(o => o.orderDetails.find(od => od.partNumber === e.partNumber));
      if (discOrder && discOrder != undefined) {
        let discItem = discOrder.orderDetails.find(e => e.partNumber == e.partNumber);
        if (discItem && discItem != undefined) {
          e.discountRecord = discOrder.discount;
          let discountAmount = (discOrder.discount * e.discountedPrice / 100);
          e.discountedPrice = e.discountedPrice - discountAmount;
          recordDiscountAmount += discountAmount * e.orderQuantity;
        }
      }
      
      if (restockingFee > 0) {
        restockingAmount = (restockingFee * e.discountedPrice / 100) * e.orderQuantity;
        e.restockingFee = restockingFee;
        e.restockingAmount = restockingAmount
        e.totalAmount = (e.discountedPrice * e.orderQuantity) - restockingAmount;
      }
      else {
        e.totalAmount = e.discountedPrice * e.orderQuantity;
      }
      
      summarySubTotal += e.wholesalePrice * e.orderQuantity;
      summaryRestockingAmount += restockingAmount;
      summaryTotal += e.totalAmount;
    });

    this.form.get('summarySubTotal').setValue(summarySubTotal.toFixed(2));
    this.form.get('summaryDiscountAmount').setValue((orderDiscountAmount + recordDiscountAmount).toFixed(2));
    this.form.get('restockingAmount').setValue(summaryRestockingAmount.toFixed(2));
    this.form.get('summaryTotal').setValue((Number(this.form.value.summarySubTotal) - Number(this.form.value.summaryDiscountAmount) - 
                                            Number(this.form.value.restockingAmount) + Number(this.form.value.summaryTax)).toFixed(2));
  }

  handleRestockingFeeOnChangeEvent() {
    let restockingFee = this.form.value.restockingFee ? Number(this.form.value.restockingFee) : 0; // Fee is now percentage
    let summaryRestockingAmount = 0;
    this.form.get('restockingAmount').setValue(0);

    this.orderDetailsList.forEach(e => {
      let restockingAmount = 0;
      //Reset Values before processing
      e.restockingFee = 0;
      e.restockingAmount = 0;
      e.totalAmount = 0; 
      
      if (restockingFee > 0) {
        restockingAmount = (restockingFee * e.discountedPrice / 100) * e.orderQuantity;
        e.restockingFee = restockingFee;
        e.restockingAmount = restockingAmount
        e.totalAmount = (e.discountedPrice * e.orderQuantity) - restockingAmount;
      }
      else {
        e.totalAmount = e.discountedPrice * e.orderQuantity;
      }
      
      summaryRestockingAmount += restockingAmount;
    });

    this.form.get('restockingAmount').setValue(summaryRestockingAmount.toFixed(2));
    this.form.get('summaryTotal').setValue((Number(this.form.value.summarySubTotal) - Number(this.form.value.summaryDiscountAmount) - 
                                            Number(this.form.value.restockingAmount) + Number(this.form.value.summaryTax)).toFixed(2));
  }

  cancel() {
    this.dialogRef.close(undefined);
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  formatNumber(amount: number) {
    return (amount) ? amount.toFixed(0) : '0';
  }

  displayPaymentHistory(row: PaymentHistoryDTO) {
    let result = '';
    result += 'Payment Date: ' + this.formatDate(row.paymentDate) + ' - ';
    result += row.invoiceBalance > 0 ? 'Partially Paid: ' : 'Fully Paid: ';
    result += row.paymentType + ' $' + this.formatCurrency(row.invoicePaymentAmount);
    result += row.customerCreditAmountUsed > 0 ? ' Credit: $' + this.formatCurrency(row.customerCreditAmountUsed) : '';
    result += row.invoiceBalance > 0 ? ' Balance: $' + this.formatCurrency(row.invoiceBalance) : '';
    return result;
  }

  creditMemo() {
    this.selection.selected.forEach(e => {
      //alert(e.partDescription);
    })
  }

  typeChange() {
    this.isDisplayDelivery = false;
    
    if (this.form.value.returnType === 2) {
      this.isDisplayDelivery = true;
      this.form.get('deliveryDate').setValue(undefined);
      this.form.get('deliveryRoute').setValue(undefined);
    }
    else {
      this.form.get('deliveryDate').setValue(this.defaults.deliveryDate);
      this.form.get('deliveryRoute').setValue(this.defaults.deliveryRoute);
    }
  }

  reasonChange() {
    this.isDisplayReasonNotes = false;
    this.cd.detectChanges();
    
    if (this.form.value.returnReason === 2 || this.form.value.returnReason === 7) {
      this.isDisplayReasonNotes = true;
    }
  }

  handleQuantityOnChangeEvent(row: OrderDetail, event: any) {
    let orderDetail = this.orderDetailsList.find(e => e.partNumber === row.partNumber);
    orderDetail.orderQuantity = event.target.value;

    if (orderDetail.orderQuantity <= orderDetail.onHandQuantity) {
      orderDetail.vendorCode = '';
      orderDetail.vendorPartNumber = '';
      orderDetail.vendorPrice = 0;
      orderDetail.vendorOnHand = 0;
    }

    orderDetail.totalAmount = orderDetail.orderQuantity * orderDetail.discountedPrice;
    this.handleSummaryOnChangeEvent2();
    this.handleRestockingFeeOnChangeEvent();
    this.cd.detectChanges();
  }

  handleSummaryOnChangeEvent2() {
    this.orderDetailsList.forEach(e => {
      let totalPrice = e.wholesalePrice * e.orderQuantity;
      e.discountedPrice = e.wholesalePrice - (e.wholesalePrice / 100 * this.form.value.summaryDiscount);
      e.totalAmount = totalPrice - (totalPrice / 100 * this.form.value.summaryDiscount);
    });

    let discountAmount = (this.orderDetailsList.length > 0) ? this.orderDetailsList.map(e => e.discountedPrice * e.orderQuantity).reduce(function (prev, next) { return prev + next }) : 0.00;
    let subTotalAmount = (this.orderDetailsList.length > 0) ? this.orderDetailsList.map(e => e.wholesalePrice * e.orderQuantity).reduce(function (prev, next) { return prev + next }) : 0;
    let totalAmount = (this.orderDetailsList.length > 0) ? this.orderDetailsList.map(e => e.totalAmount).reduce(function (prev, next) { return prev + next }) : 0;
    let totalTax = (this.form.value.summaryTaxRate > 0) ? totalAmount * this.form.value.summaryTaxRate / 100 : 0.00;

    this.summaryDiscount = this.form.value.summaryDiscount;
    this.summarySubTotal = subTotalAmount; // - totalTax;
    this.summaryTax = totalTax;
    this.summaryTaxRate = this.form.value.summaryTaxRate;
    this.summaryTotal = (this.orderDetailsList.length > 0) ? totalAmount + totalTax : 0 + totalTax;

    this.form.get('summaryDiscountAmount').setValue((subTotalAmount - discountAmount).toFixed(2));
    this.form.get('summarySubTotal').setValue(this.summarySubTotal.toFixed(2));
    this.form.get('summaryTax').setValue(this.summaryTax.toFixed(2));
    this.form.get('summaryTotal').setValue(this.summaryTotal.toFixed(2));
  }
}
