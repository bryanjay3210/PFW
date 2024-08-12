import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { Order, PartsPickingDetail, PartsPicking, Role, User, Vendor, StockOrderDetailDTO } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { MatTableDataSource } from '@angular/material/table';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { SelectionModel } from '@angular/cdk/collections';
import { PartsPickingService } from 'src/services/partspicking.service';

@Component({
  selector: 'vex-parts-picking-create-update',
  templateUrl: './parts-picking-create-update.component.html',
  styleUrls: ['./parts-picking-create-update.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class PartsPickingCreateUpdateComponent implements OnInit {
  columns: TableColumn<StockOrderDetailDTO>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Order Qty', property: 'orderQuantity', type: 'text', visible: true },
    { label: 'Order Number', property: 'orderNumber', type: 'text', visible: true},
    { label: 'PO Number', property: 'purchaseOrderNumber', type: 'text', visible: false},
    { label: 'Part Number', property: 'partNumber', type: 'text', visible: true },
    { label: 'Parts Link', property: 'mainPartsLinkNumber', type: 'text', visible: true },
    //{ label: 'Stock Qty', property: 'stockQuantity', type: 'text', visible: true },
    { label: 'Location', property: 'stockLocation', type: 'badge', visible: true },
    { label: 'CustomerName', property: 'customerName', type: 'text', visible: true },
    { label: 'Ship Zone', property: 'shipZone', type: 'text', visible: true },
    { label: 'Delivery Method', property: 'deliveryMethod', type: 'text', visible: true },
    { label: 'Category', property: 'sequence', type: 'text', visible: true },
    { label: 'Part Description', property: 'partDescription', type: 'text', visible: true },
    { label: 'Order Date', property: 'orderDate', type: 'text', visible: true },
  ];

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";
  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  dataSource: MatTableDataSource<StockOrderDetailDTO> | null;
  selection = new SelectionModel<StockOrderDetailDTO>(true, []);
  roleList: Role[];
  userTypeList: Lookup[];
  accessTypeList: Lookup[];
  vendorList: Vendor[];
  vendorOrderList: Order[] = [];
  partsPickingDetailList: PartsPickingDetail[] = [];

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
  warehouseFilter: number = 0;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<PartsPickingCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private partsPickingService: PartsPickingService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PartsPicking);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();

    if (this.defaults.details) {
      this.columns.push({ label: 'Status', property: 'ppDetailStatus', type: 'text', visible: true });
      this.columns.push({ label: 'Actions', property: 'actions', type: 'button', visible: true });
      this.mode = 'update';
      this.dataSource.data = this.defaults.details.partsPickingDetails;
    } else {
      this.warehouseFilter = this.defaults.warehouse;
      this.defaults = {} as PartsPicking;
      this.getData();
    }

    this.form = this.fb.group({
      id: [PartsPickingCreateUpdateComponent.id++],
      pickNumber: [(this.defaults.pickNumber) || ''],
    });
  }

  getCurrentVendor(vendorId: number) {
    this.currentVendor =  this.vendorList.find(e => e.id === vendorId);
  }
  
  getData() {
    this.getStockOrderDetails();
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
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  selectInvoice(vendorOrder: StockOrderDetailDTO) {
    this.selection.toggle(vendorOrder);
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("Parts Picking").then(answer => {
          if (!answer.isConfirmed) return;
            this.createPartsPicking();
        });
      }
      else this.alertService.validationNotification("Parts Picking");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("Parts Picking").then(answer => {
          if (!answer.isConfirmed) return;
            this.updatePartsPicking();
        });
      }
      else this.alertService.validationNotification("Parts Picking");
    }
  }

  createPartsPicking() {
    const partsPicking = {} as PartsPicking;
    this.mapFormValuesToPartsPicking(partsPicking);
    this.dialogRef.close(partsPicking);
  }

  updatePartsPicking() {
    const partsPicking = {} as PartsPicking;
    this.mapFormValuesToPartsPicking(partsPicking);
    this.dialogRef.close(partsPicking);
  }

  mapFormValuesToPartsPicking(partsPicking: PartsPicking) {
    partsPicking.pickNumber = this.isCreateMode() ? '' : this.defaults.details.pickNumber;
    partsPicking.partsPickingDate = this.isCreateMode() ? moment(new Date()) : this.defaults.details.partsPickingDate;
    partsPicking.isPrinted = this.isCreateMode() ? false : this.defaults.details.isPrinted;
    partsPicking.statusId = this.isCreateMode() ? 1 : this.defaults.details.statusId;
    partsPicking.pickStatus = this.isCreateMode() ? 'Picking Stage' : this.defaults.details.pickStatus;

    partsPicking.isActive = this.isCreateMode() ? true : this.defaults.details.isActive;
    partsPicking.isDeleted = this.isCreateMode() ? false : this.defaults.details.isDeleted;
    partsPicking.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.details.createdBy;
    partsPicking.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.details.createdDate;

    if (this.isUpdateMode()) {
      partsPicking.modifiedBy = this.currentUser.userName;
      partsPicking.modifiedDate = moment(new Date());
      partsPicking.id = this.defaults.details.id;
    }

    if (this.isCreateMode()) {
      let selectedRecords = this.selection.selected.sort((a, b) => a.orderNumber - b.orderNumber);
      selectedRecords.forEach(e => {
        let ordDate = new Date(e.orderDate.toLocaleString()).toISOString();
        let delDate = new Date(e.deliveryDate.toLocaleString()).toISOString();
        const partsPickingDetail = {} as PartsPickingDetail;
        partsPickingDetail.orderDetailId = e.id;
        partsPickingDetail.productId = e.productId
        partsPickingDetail.orderId = e.orderId;
        partsPickingDetail.orderNumber = e.orderNumber;
        partsPickingDetail.orderQuantity = e.orderQuantity;
        partsPickingDetail.stockQuantity = e.stockQuantity;
        partsPickingDetail.stockLocation = e.stockLocation;
        partsPickingDetail.shipZone = e.shipZone;
        partsPickingDetail.partNumber = e.partNumber;
        partsPickingDetail.sequence = e.sequence;
        partsPickingDetail.partDescription = e.partDescription;
        partsPickingDetail.orderDate = moment(ordDate);
        partsPickingDetail.deliveryDate = moment(e.deliveryDate);
        partsPickingDetail.deliveryRoute = e.deliveryRoute;
        partsPickingDetail.statusId = 1;
        partsPickingDetail.ppDetailStatus = 'Picking Stage';
        partsPickingDetail.deliveryMethod = e.deliveryMethod;
        partsPickingDetail.mainPartsLinkNumber = e.mainPartsLinkNumber;
        partsPickingDetail.purchaseOrderNumber = e.purchaseOrderNumber;
        partsPickingDetail.customerName = e.customerName;
        partsPickingDetail.isActive = partsPicking.isActive;
        partsPickingDetail.isDeleted = partsPicking.isDeleted;
        partsPickingDetail.createdBy = partsPicking.createdBy;
        partsPickingDetail.createdDate = partsPicking.createdDate;

        this.partsPickingDetailList.push(partsPickingDetail);
      })
    }
    else {
      let selectedRecords = this.dataSource.data as PartsPickingDetail[];
      selectedRecords.forEach(e => {
        let partsPickingDetail = e;
        this.partsPickingDetailList.push(partsPickingDetail);
      });
    }

    partsPicking.partsPickingDetails = this.partsPickingDetailList;
  }

  printPartsPicking(event: any, row: PartsPicking) {
    if (event) {
      event.stopPropagation();
    }
    // this.data = this.mapRowTodata(row);
    // this.cd.detectChanges();
    // setTimeout( () => { 
    //   window.print(); 
    // }, 2000);
  }

  pickPart(event: any, row: PartsPickingDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.actionNotification('Tag', 'Parts Picking Detail').then(answer => {
      if (!answer.isConfirmed) return;

      let location = row.warehouseLocations.find(e => e.location === row.stockLocation);
      if (location === undefined) {
        this.alertService.requiredNotification('Please select a location prior to picking.');
        return;
      }

      row.statusId = 2;
      row.ppDetailStatus = 'Picked at ' + row.stockLocation;
    });
  }

  skipPart(event: any, row: PartsPickingDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.actionNotification('Tag', 'Parts Picking Detail').then(answer => {
      if (!answer.isConfirmed) return;

      // Commented this part to allow Skipping Item on zero Stock Quantity
      // let location = row.warehouseLocations.find(e => e.location === row.stockLocation);
      // if (location === undefined) {
      //   this.alertService.requiredNotification('Please select a location prior to skipping.');
      //   return;
      // }

      row.statusId = 3;
      row.ppDetailStatus = 'Skipped Item';
    });
  }

  deletePartsPickingDetail(event: any, row: PartsPickingDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.deleteNotification('Parts Picking Detail').then(answer => {
      if (!answer.isConfirmed) return;

      this.partsPickingService.softDeletePartsPickingDetail(row).subscribe(result => {
        if (result) {
          let tempData =  this.dataSource.data;
          tempData.splice(this.dataSource.data.findIndex(e => e.id === row.id), 1);
          this.dataSource.data = tempData;
          this.alertService.successNotification("Parts Picking Detail", "Delete");
          this.cd.detectChanges();
        }
        else {
          this.alertService.failNotification("Parts Picking Detail", "Delete");
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

  getStockOrderDetails() {
    if (this.isUpdateMode()) return;

    this.alertService.showBlockUI('Loading Order Details');
    this.partsPickingService.getStockOrderDetails(this.warehouseFilter).subscribe(result => {
      if (result) {
        this.dataSource.data = result;
        this.cd.detectChanges();
        this.alertService.hideBlockUI();
      }
    });
  }

  locationSelectionChange(event: any, row: any) {
    let location = row.warehouseLocations.find(e => e.location === event.value);
    if (location !== undefined) {
      row.stockQuantity = location.quantity;
    }
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }
}