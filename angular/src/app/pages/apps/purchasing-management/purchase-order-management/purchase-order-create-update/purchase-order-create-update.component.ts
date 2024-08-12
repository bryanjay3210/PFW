import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { Order, PurchaseOrderDetail, PurchaseOrder, Role, User, Vendor, VendorOrderDTO } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { MatTableDataSource } from '@angular/material/table';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { SelectionModel } from '@angular/cdk/collections';
import { VendorService } from 'src/services/vendor.service';
import { PurchaseOrderService } from 'src/services/purchaseorder.service';

@Component({
  selector: 'vex-purchase-order-create-update',
  templateUrl: './purchase-order-create-update.component.html',
  styleUrls: ['./purchase-order-create-update.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class PurchaseOrderCreateUpdateComponent implements OnInit {
  columns: TableColumn<VendorOrderDTO>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Quantity', property: 'orderQuantity', type: 'text', visible: true },
    { label: 'Order #', property: 'orderNumber', type: 'text', visible: true},
    { label: 'PO Number', property: 'purchaseOrderNumber', type: 'text', visible: false},
    { label: 'Part Number', property: 'partNumber', type: 'text', visible: true },
    { label: 'Parts Link', property: 'mainPartsLinkNumber', type: 'text', visible: true },
    { label: 'Vendor Part #', property: 'vendorPartNumber', type: 'text', visible: true },
    { label: 'Vendor Price', property: 'vendorPrice', type: 'number', visible: true },
    { label: 'Ship Zone', property: 'shipZone', type: 'text', visible: true },
    { label: 'Delivery Method', property: 'deliveryMethod', type: 'text', visible: true },
    { label: 'Category', property: 'sequence', type: 'text', visible: true },
    { label: 'Part Description', property: 'partDescription', type: 'text', visible: true },
    { label: 'Order Date', property: 'orderDate', type: 'text', visible: true },
    { label: 'Received Date', property: 'receivedDate', type: 'text', visible: true },
    { label: 'Customer Name', property: 'customerName', type: 'text', visible: true },
    { label: 'Status', property: 'poDetailStatus', type: 'text', visible: true },
  ];

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";
  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  dataSource: MatTableDataSource<VendorOrderDTO> | null;
  selection = new SelectionModel<VendorOrderDTO>(true, []);
  roleList: Role[];
  userTypeList: Lookup[];
  accessTypeList: Lookup[];
  vendorList: Vendor[];
  vendorOrderList: Order[] = [];
  purchaseOrderDetailList: PurchaseOrderDetail[] = [];

  todayDate: Date = new Date();
  visible = false;
  isUseCredit: boolean = false;
  isCashOrCreditSelected: boolean = true;
 
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  currentVendor: Vendor = undefined;
  creditBalance: number = 0;
  totalAmountCM: number = 0;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: PurchaseOrder,
    private dialogRef: MatDialogRef<PurchaseOrderCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private purchaseOrderService: PurchaseOrderService,
    private vendorService: VendorService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PurchaseOrderManagement);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();

    if (this.defaults) {
      this.columns.push({ label: 'Actions', property: 'actions', type: 'button', visible: true });
      this.mode = 'update';
      this.dataSource.data = this.defaults.purchaseOrderDetails;
    } else {
      this.defaults = {} as PurchaseOrder;
    }

    this.getData();

    this.form = this.fb.group({
      id: [PurchaseOrderCreateUpdateComponent.id++],
      vendorName: ['', Validators.required],
      vendorCode: [this.defaults.vendorCode || '', Validators.required],
      vendorPO: [(this.defaults.vendorPO) || ''],
      pfwbNumber: [(this.defaults.pfwbNumber) || ''],
      totalAmount: [this.formatCurrency(this.defaults.totalAmount) || '0.00'],
      totalQuantity: [(this.defaults.totalQuantity) || '0'],
    });
  }

  getCurrentVendor(vendorId: number) {
    this.currentVendor =  this.vendorList.find(e => e.id === vendorId);
    this.updateVendorDetails();
  }
  
  getData() {
    this.vendorService.getVendors().subscribe((result: Vendor[]) => {
      if (result) {
        this.vendorList = result;
        if (this.isUpdateMode()) {
          this.getCurrentVendor(this.defaults.vendorId);
        }
        this.cd.detectChanges();
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
    this.getTotals();
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  selectInvoice(vendorOrder: VendorOrderDTO) {
    this.selection.toggle(vendorOrder);
    this.getTotals();
  }

  getTotals() {
    if (this.isUpdateMode()) return;
    this.form.get('totalAmount').setValue('0.00');
    this.form.get('totalQuantity').setValue(0);

    if (this.selection.selected.length > 0) {
      let amt = this.selection.selected.map(e => e.vendorPrice).reduce(function(a, b){ return a + b; });
      let qty = this.selection.selected.map(e => e.orderQuantity).reduce(function(a, b){ return a + b; });
  
      this.form.get('totalAmount').setValue(this.formatCurrency(amt));
      this.form.get('totalQuantity').setValue(qty);
    }
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("Purchase Order").then(answer => {
          if (!answer.isConfirmed) return;
            this.createPurchaseOrder();
        });
      }
      else this.alertService.validationNotification("Purchase Order");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("Purchase Order").then(answer => {
          if (!answer.isConfirmed) return;
            this.updatePurchaseOrder();
        });
      }
      else this.alertService.validationNotification("Purchase Order");
    }
  }

  createPurchaseOrder() {
    const purchaseOrder = {} as PurchaseOrder;
    this.mapFormValuesToPurchaseOrder(purchaseOrder);
    this.dialogRef.close(purchaseOrder);
  }

  updatePurchaseOrder() {
    const purchaseOrder = {} as PurchaseOrder;
    this.mapFormValuesToPurchaseOrder(purchaseOrder);
    this.dialogRef.close(purchaseOrder);
  }

  mapFormValuesToPurchaseOrder(purchaseOrder: PurchaseOrder) {
    purchaseOrder.vendorId = this.currentVendor.id;
    purchaseOrder.vendorName = this.currentVendor.vendorName;
    purchaseOrder.vendorCode = this.currentVendor.vendorCode;
    purchaseOrder.vendorPO = this.form.value.vendorPO;
    purchaseOrder.purchaseOrderDate = this.isCreateMode() ? moment(new Date()) : this.defaults.purchaseOrderDate;
    purchaseOrder.isPrinted = this.isCreateMode() ? false : this.defaults.isPrinted;
    
    purchaseOrder.poStatus = this.isCreateMode() ? 'Ordered' : this.defaults.poStatus;
    purchaseOrder.statusId = this.isCreateMode() ? 1 : this.defaults.statusId;

    purchaseOrder.isActive = this.isCreateMode() ? true : this.defaults.isActive;
    purchaseOrder.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    purchaseOrder.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
    purchaseOrder.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;

    if (this.isUpdateMode()) {
      purchaseOrder.pfwbNumber = this.defaults.pfwbNumber;
      purchaseOrder.modifiedBy = this.currentUser.userName;
      purchaseOrder.modifiedDate = moment(new Date());
      purchaseOrder.id = this.defaults.id;
      if (this.defaults.receivedDate !== null) {
        purchaseOrder.receivedDate = moment(new Date(this.defaults.receivedDate.toLocaleString()));
      }
    }

    if (this.isCreateMode()) {
      let selectedRecords = this.selection.selected.sort((a, b) => a.orderNumber - b.orderNumber);
      selectedRecords.forEach(e => {
        let ordDate = new Date(e.orderDate.toLocaleString()).toISOString();
        let delDate = new Date(e.deliveryDate.toLocaleString()).toISOString();
        const purchaseOrderDetail = {} as PurchaseOrderDetail;
        purchaseOrderDetail.orderId = e.orderId;
        purchaseOrderDetail.orderDetailId = e.id;
        purchaseOrderDetail.orderNumber = e.orderNumber;
        purchaseOrderDetail.purchaseOrderNumber = e.purchaseOrderNumber;
        purchaseOrderDetail.orderQuantity = e.orderQuantity;
        purchaseOrderDetail.shipZone = e.shipZone;
        purchaseOrderDetail.partNumber = e.partNumber;
        purchaseOrderDetail.vendorPartNumber = e.vendorPartNumber;
        purchaseOrderDetail.vendorPrice = e.vendorPrice;
        purchaseOrderDetail.sequence = e.sequence;
        purchaseOrderDetail.partDescription = e.partDescription;
        purchaseOrderDetail.orderDate = moment(ordDate);
        
        purchaseOrderDetail.poDetailStatus = 'Ordered';
        purchaseOrderDetail.statusId = 1;

        purchaseOrderDetail.isActive = purchaseOrder.isActive;
        purchaseOrderDetail.isDeleted = purchaseOrder.isDeleted;
        purchaseOrderDetail.createdBy = purchaseOrder.createdBy;
        purchaseOrderDetail.createdDate = purchaseOrder.createdDate;
        purchaseOrderDetail.mainPartsLinkNumber = e.mainPartsLinkNumber;
        purchaseOrderDetail.deliveryMethod = e.deliveryMethod;
        purchaseOrderDetail.customerName = e.customerName;
        purchaseOrderDetail.deliveryDate = moment(e.deliveryDate);
        purchaseOrderDetail.deliveryRoute = e.deliveryRoute;

        this.purchaseOrderDetailList.push(purchaseOrderDetail);
      })
    }
    else {
      let selectedRecords = this.dataSource.data as PurchaseOrderDetail[];
      selectedRecords.forEach(e => {
        let purchaseOrderDetail = e;

        if (purchaseOrderDetail.statusId === 2) {
          purchaseOrderDetail.receivedDate =  purchaseOrder.modifiedDate;
          if (!this.defaults.receivedDate || this.defaults.receivedDate === null || this.defaults.receivedDate === undefined)
          {
            purchaseOrder.receivedDate =  purchaseOrder.modifiedDate;
          }
        }
        
        purchaseOrderDetail.modifiedBy = purchaseOrder.modifiedBy;
        purchaseOrderDetail.modifiedDate = purchaseOrder.modifiedDate;

        this.purchaseOrderDetailList.push(purchaseOrderDetail);
      });
    }

    purchaseOrder.purchaseOrderDetails = this.purchaseOrderDetailList;

    let ordered = purchaseOrder.purchaseOrderDetails.filter(e => e.statusId === 1);
    if (ordered && ordered.length > 0) {
      if (ordered.length !== purchaseOrder.purchaseOrderDetails.length) {
        purchaseOrder.statusId = 3;
        purchaseOrder.poStatus = 'Partially Received';
      }
    }
    else {
      purchaseOrder.statusId = 2;
      purchaseOrder.poStatus = 'Received';
    }
  }

  handleVendorPriceOnChangeEvent(row: PurchaseOrderDetail, event: any) {
    let purchaseOrderDetail = this.dataSource.data.find(e => e.id === row.id);
    purchaseOrderDetail.vendorPrice = Number(event.target.value);
    
    this.form.get('totalAmount').setValue('0.00');
    this.form.get('totalQuantity').setValue(0);
    let amt = this.dataSource.data.map(e => e.vendorPrice).reduce(function(a, b){ return a + b; });
    let qty = this.dataSource.data.map(e => e.orderQuantity).reduce(function(a, b){ return a + b; });
    this.form.get('totalAmount').setValue(this.formatCurrency(amt));
    this.form.get('totalQuantity').setValue(qty);

    this.cd.detectChanges();
  }

  printPurchaseOrder(event: any, row: PurchaseOrder) {
    if (event) {
      event.stopPropagation();
    }
    // this.data = this.mapRowTodata(row);
    // this.cd.detectChanges();
    // setTimeout( () => { 
    //   window.print(); 
    // }, 2000);
  }
  receivePurchaseOrderDetail(event: any, row: PurchaseOrderDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.actionNotification('Receive', 'Purchase Order Detail').then(answer => {
      if (!answer.isConfirmed) return;
      
      row.statusId = 2;
      row.poDetailStatus = "Received";
      this.cd.detectChanges();
    });
  }

  returnPurchaseOrderDetail(event: any, row: PurchaseOrderDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.actionNotification('Return', 'Purchase Order Detail').then(answer => {
      if (!answer.isConfirmed) return;

      row.statusId = 3;
      row.poDetailStatus = "Returned to Vendor";
      this.cd.detectChanges();
    });
  }

  creditPurchaseOrderDetail(event: any, row: PurchaseOrderDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.actionNotification('Credit', 'Purchase Order Detail').then(answer => {
      if (!answer.isConfirmed) return;

      row.statusId = 4;
      row.poDetailStatus = "Credited by Vendor";
      this.cd.detectChanges();
    });
  }

  deletePurchaseOrderDetail(event: any, row: PurchaseOrderDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.deleteNotification('Purchase Order Detail').then(answer => {
      if (!answer.isConfirmed) return;

      this.purchaseOrderService.softDeletePurchaseOrderDetail(row).subscribe(result => {
        if (result) {
          let tempData =  this.dataSource.data;
          tempData.splice(this.dataSource.data.findIndex(e => e.id === row.id), 1);
          this.dataSource.data = tempData;
          
          this.form.get('totalAmount').setValue('0.00');
          this.form.get('totalQuantity').setValue(0);
          let amt = this.dataSource.data.map(e => e.vendorPrice).reduce(function(a, b){ return a + b; });
          let qty = this.dataSource.data.map(e => e.orderQuantity).reduce(function(a, b){ return a + b; });
          this.form.get('totalAmount').setValue(this.formatCurrency(amt));
          this.form.get('totalQuantity').setValue(qty);
          
          this.alertService.successNotification("Purchase Order Detail", "Delete");
          this.cd.detectChanges();
        }
        else {
          this.alertService.failNotification("Purchase Order Detail", "Delete");
        }
      });
    });
  }



  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  getVendorDetails(event: any) {
    if (this.isUpdateMode()) return;
    this.selection.clear();
    this.getTotals();
    this.alertService.showBlockUI('Loading Order Details');
    this.currentVendor = this.vendorList.find(e => e.id === event.value);
    this.updateVendorDetails();
    this.vendorService.getVendorOrdersByVendorCode(this.currentVendor.vendorCode).subscribe(result => {
      if (result) {
        this.dataSource.data = result;
        this.cd.detectChanges();
        this.alertService.hideBlockUI();
      }
    });
  }

  updateVendorDetails() {
    if (this.isCreateMode()) {
      this.form.get('vendorName').setValue(this.currentVendor.id);
      this.form.get('vendorCode').setValue(this.currentVendor.vendorCode);
      this.form.get('vendorPO').setValue('');
      this.form.get('pfwbNumber').setValue('');
    }
    else {
      this.form.get('vendorName').setValue(this.currentVendor.id);
      this.form.get('vendorCode').setValue(this.currentVendor.vendorCode);
      this.form.get('vendorPO').setValue(this.defaults.vendorPO);
      this.form.get('pfwbNumber').setValue(this.defaults.pfwbNumber);
    }
  }

  clearVendorDetails() {
    this.form.get('vendorName').setValue(0);
    this.form.get('vendorCode').setValue('');
    this.form.get('vendorPO').setValue('');
    this.form.get('pfwbNumber').setValue('');
  }

  formatDate(orderDate: moment.Moment) {
    return orderDate ? moment(orderDate).format('MM/DD/YYYY h:mm A') : '';
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }
}