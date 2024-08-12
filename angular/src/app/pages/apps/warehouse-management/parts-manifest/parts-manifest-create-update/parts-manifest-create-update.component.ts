import { ChangeDetectorRef, Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { Order, PartsManifestDetail, PartsManifest, Role, User, Vendor, StockOrderDetailDTO, Driver, Product } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { MatTableDataSource } from '@angular/material/table';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { SelectionModel } from '@angular/cdk/collections';
import { PartsManifestService } from 'src/services/partsmanifest.service';
import { DriverService } from 'src/services/driver.service';
import { error } from 'console';
import { OrderService } from 'src/services/order.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ProductService } from 'src/services/product.service';
import { SearchProductListComponent } from '../../put-away/search-product-list/search-product-list.component';
import { VendorService } from 'src/services/vendor.service';
// import { PartsManifestDetailService } from 'src/services/partsmanifestdetail.service';

@Component({
  selector: 'vex-parts-manifest-create-update',
  templateUrl: './parts-manifest-create-update.component.html',
  styleUrls: ['./parts-manifest-create-update.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class PartsManifestCreateUpdateComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('purpose', { static: false }) purpose: ElementRef;
  @ViewChild('vendor', { static: false }) vendor: ElementRef;
  @ViewChild('driver', { static: false }) driver: ElementRef;
  @ViewChild('product', { static: false }) product: ElementRef;

  columns: TableColumn<any>[] = [];
  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";
  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  dataSource: MatTableDataSource<any> | null;

  selection = new SelectionModel<PartsManifestDetail>(true, []);
  roleList: Role[];
  userTypeList: Lookup[];
  accessTypeList: Lookup[];
  
  currentDriver: Driver = undefined;
  currentOrder: Order = undefined;
  currentPartsManifest: PartsManifest = undefined;

  partsManifestDetailList: PartsManifestDetail[] = [];

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

  driverCtrl = new UntypedFormControl();
  productCtrl = new UntypedFormControl();
  currentOrderNumber: number = 0;
  currentShipAddressName: string = '';
  currentPaymentTermName: string = '';
  totalStops: number = 0;

  purposeCtrl = new UntypedFormControl();
  purposeList: any = [
    {'id': '1', 'name': 'Transfer'}, 
    {'id': '2', 'name': 'Vendor Return'},
    {'id': '3', 'name': 'Trash'}
  ];

  vendorCtrl = new UntypedFormControl();
  vendorList: Vendor[] = [];
  // any = [
  //   {'id': '1', 'name': 'PROA'}, 
  //   {'id': '2', 'name': 'JCAP'},
  //   {'id': '3', 'name': 'CAR'}
  // ];

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: PartsManifest,
    private dialogRef: MatDialogRef<PartsManifestCreateUpdateComponent>,
    private dialog: MatDialog,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private driverService: DriverService,
    private productService: ProductService,
    private vendorService: VendorService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PartsManifest);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();
    this.currentOrderNumber = 0;
    this.currentShipAddressName = '';
    this.currentPaymentTermName = '';
    if (this.defaults) {
      this.mode = 'update';
      this.currentPartsManifest = this.defaults;
      this.setTableColumns();
      this.dataSource.data = this.defaults.partsManifestDetails;
      this.getTotalStops();
    } else {
      this.getVendors();
      this.mode = 'create';
      this.setTableColumns();
      this.dataSource.data = [];
      this.defaults = {} as PartsManifest;
    }

    this.form = this.fb.group({
      id: [PartsManifestCreateUpdateComponent.id++],
      partsManifestNumber: [(this.defaults.partsManifestNumber) || ''],
    });
  }

  getVendors() {
    this.alertService.showBlockUI('Loading Vendors...');
    this.vendorService.getVendors().subscribe((result: Vendor[]) => {
      this.vendorList = result;
      this.alertService.hideBlockUI();
    });
  }

  setTableColumns() {
    if (this.isUpdateMode()) {
      this.columns = [
        { label: 'Part Number', property: 'partNumber', type: 'text', visible: true },
        { label: 'Part Description', property: 'partDescription', type: 'text', visible: true },
        { label: 'Quantity', property: 'quantity', type: 'text', visible: true },
        { label: 'Actions', property: 'actions', type: 'button', visible: true },
      ];
    }
    else if(this.isCreateMode()) {
      this.columns = [
        { label: 'Part Number', property: 'partNumber', type: 'text', visible: true },
        { label: 'Part Description', property: 'partDescription', type: 'text', visible: true },
        { label: 'Quantity', property: 'quantity', type: 'text', visible: true },
        { label: 'Actions', property: 'actions', type: 'button', visible: true },
      ];
    }
  }

  searchDriver() {
    if (!this.purposeCtrl.value) {
      this.alertService.requiredNotification('Please input Purpose prior to adding Driver.')
      this.vendorCtrl.setValue(undefined);
      this.driverCtrl.setValue(undefined);
      this.purpose.nativeElement.focus();
      this.cd.detectChanges();
      return;
    }

    if (this.purposeCtrl.value === '2') { //Vendor Return
      if (!this.vendorCtrl.value) {
        this.alertService.requiredNotification('Please input Vendor prior to adding Driver.')
        this.driverCtrl.setValue(undefined);
        this.vendor.nativeElement.focus();
        this.cd.detectChanges();
      }
    }

    this.driverService.getDriverByDriverNumber(this.driverCtrl.value).subscribe((result: Driver) => {
      if (result) {
        this.currentDriver = result;
        this.addPartsManifest();
        this.product.nativeElement.focus();
      }
      else {
        this.alertService.failNotification('Driver', 'Search');
        this.driver.nativeElement.focus();
      }
    },
    error => {
      this.alertService.failNotification('Driver', 'Search');
      this.driver.nativeElement.focus();
    });
  }

  searchProduct() {
    if (!this.purposeCtrl.value) {
      this.alertService.requiredNotification('Please select Purpose prior to adding Products.')
      this.productCtrl.setValue(undefined);
      this.vendorCtrl.setValue(undefined);
      this.driverCtrl.setValue(undefined);
      this.productCtrl.setValue(undefined);
      this.purpose.nativeElement.focus();
      this.cd.detectChanges();
      return;
    }

    if (!this.currentDriver) {
      this.alertService.requiredNotification('Please input Driver prior to adding Products.')
      this.productCtrl.setValue(undefined);
      this.driver.nativeElement.focus();
      this.cd.detectChanges();
      return;
    }

    this.productService.getProductByPartNumber(this.productCtrl.value).subscribe(result => {
      if (result && result.length > 0) {
        if (result.length > 1)
        {
          // Show Popup
          this.dialog.open(SearchProductListComponent, {
            height: '60%',
            width: '60%',
            data: result
          }).afterClosed().subscribe((product: Product[]) => {
            if (product && product.length > 0) {
              this.AddProductToList(product);
            }
          });
        }
        else {
          this.AddProductToList(result);
        }
      }
      else {
        this.alertService.failNotification('Product', 'Search');
      }
    });
  }

  private AddProductToList(products: Product[]) {
    products.forEach((product: Product) => {
      let exist = this.partsManifestDetailList.find(e => e.productId === product.id);
      if (exist) {
        exist.quantity += 1;
      }
      else {
        const detail = {} as PartsManifestDetail;
        detail.partDescription = product.partDescription;
        detail.partNumber = product.partNumber;
        detail.productId = product.id;
        detail.quantity = 1
        detail.isActive = true;
        detail.isDeleted = false;
        detail.createdBy = this.currentUser.userName;
        detail.createdDate = moment(new Date());
        // detail.modifiedBy = this.currentUser.userName;
        // detail.modifiedDate = moment(new Date());
        this.partsManifestDetailList.push(detail);
      }
    });
    

    this.dataSource.data = this.partsManifestDetailList;
    this.productCtrl.setValue(undefined);
    this.cd.detectChanges();
  }

  purposeSelectionChange(event: any) {
    this.vendorCtrl.setValue(undefined);
    this.driverCtrl.setValue(undefined);
    this.productCtrl.setValue(undefined);
  }

  vendorSelectionChange(event: any) {
    // alert(event.value)
  }

  addPartsManifest() {
    var partsManifest = {} as PartsManifest;
    let purpose = this.purposeList.find(e => e.id === this.purposeCtrl.value);
    if (purpose) {
      partsManifest.partsManifestNumber = '';
      partsManifest.purposeId = purpose.id;
      partsManifest.purposeName = purpose.name;
      partsManifest.driverId = this.currentDriver.id;
      partsManifest.driverName = this.currentDriver.firstName + ' ' + this.currentDriver.lastName;

      if (this.purposeCtrl.value === '2') { //Vendor Return
        let vendor = this.vendorList.find(e => e.id === this.vendorCtrl.value);
        if (vendor) {
          partsManifest.vendorId = vendor.id;
          partsManifest.vendorCode = vendor.vendorCode;
          partsManifest.vendorName = vendor.vendorName;
        }
      }

      partsManifest.id = 0;
      partsManifest.isActive = true;
      partsManifest.isDeleted = false;
      partsManifest.createdBy = this.currentUser.userName;
      partsManifest.createdDate = moment(new Date());
      this.currentPartsManifest = partsManifest;
      this.currentPartsManifest.partsManifestDetails = [];
    }
  }

  // addPartsManifestDetail() {
  //   // let orderDetailId = 0;
  //   this.currentOrder.orderDetails.forEach(e => {
  //     const partsManifestDetail = {} as PartsManifestDetail;
  //     // if (orderDetailId === 0) {
  //     //   partsManifestDetail.orderTotalAmount = this.currentOrder.totalAmount;
  //     //   orderDetailId = e.id;
  //     // }
  //     partsManifestDetail.createdBy = this.currentPartsManifest.createdBy;
  //     partsManifestDetail.createdDate = this.currentPartsManifest.createdDate;
  //     partsManifestDetail.partsManifestId = 0;
  //     partsManifestDetail.id = 0;
  //     partsManifestDetail.isActive = true;
  //     partsManifestDetail.isDeleted = false;
  //     // partsManifestDetail.orderDetailId = e.id;
  //     // partsManifestDetail.orderId = this.currentOrder.id;
  //     // partsManifestDetail.orderNumber = this.currentOrder.orderNumber;
  //     // partsManifestDetail.orderQuantity = e.orderQuantity;
  //     partsManifestDetail.partNumber = e.partNumber;
  //     // partsManifestDetail.paymentTermName = this.currentOrder.paymentTermName;
  //     // partsManifestDetail.shipAddressName = this.currentOrder.shipAddressName;
  //     // partsManifestDetail.statusDetail = this.currentPartsManifest.statusDetail;
  //     // partsManifestDetail.statusId = this.currentPartsManifest.statusId;
  //     // partsManifestDetail.totalAmount = e.totalAmount;
  //     this.currentPartsManifest.partsManifestDetails.push(partsManifestDetail);
  //   });

  //   this.cd.detectChanges();
  // }

  clearOrders() {
    this.alertService.clearNotification("Orders").then(answer => {
      if (!answer.isConfirmed) return;
      this.resetResults();
    });
  }

  private resetResults() {
    this.purposeCtrl.setValue('');
    this.vendorCtrl.setValue('');
    this.driverCtrl.setValue('');
    this.productCtrl.setValue('');
    this.partsManifestDetailList = [];
    this.dataSource.data = this.partsManifestDetailList;
    this.cd.detectChanges();
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

  selectPartsManifestDetail(partsManifestDetail: PartsManifestDetail) {
    this.selection.toggle(partsManifestDetail);
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("Parts Manifest").then(answer => {
          if (!answer.isConfirmed) return;
            this.createPartsManifest();
        });
      }
      else this.alertService.validationNotification("Parts Manifest");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("Parts Manifest").then(answer => {
          if (!answer.isConfirmed) return;
            this.updatePartsManifest();
        });
      }
      else this.alertService.validationNotification("Parts Manifest");
    }
  }

  createPartsManifest() {
    const partsManifest = {} as PartsManifest;
    this.mapFormValuesToPartsManifest(partsManifest);
    this.dialogRef.close(this.currentPartsManifest);
  }

  updatePartsManifest() {
    const partsManifest = {} as PartsManifest;
    this.mapFormValuesToPartsManifest(partsManifest);
    this.dialogRef.close(this.currentPartsManifest);
  }

  mapFormValuesToPartsManifest(partsManifest: PartsManifest) {
    partsManifest = this.currentPartsManifest;
    partsManifest.partsManifestDetails = this.dataSource.data;
  }

  printPartsManifest(event: any, row: PartsManifest) {
    if (event) {
      event.stopPropagation();
    }
    // this.data = this.mapRowTodata(row);
    // this.cd.detectChanges();
    // setTimeout( () => { 
    //   window.print(); 
    // }, 2000);
  }

  // retry(event: any, row: PartsManifestDetail) {
  //   if (event) {
  //     event.stopPropagation();
  //   }

  //   this.alertService.actionNotification('Retry', 'Parts Manifest Detail').then(answer => {
  //     if (!answer.isConfirmed) return;

  //     row.statusId = 2;
  //     row.statusDetail = 'Retry';
  //     row.modifiedBy = this.currentUser.userName;
  //     row.modifiedDate = moment(new Date());

  //     this.partsManifestDetailService.updatePartsManifestDetail(row).subscribe(result => {
  //       if (result) {
  //         this.alertService.successNotification('Parts Manifest Detail', 'Update');
  //       }
  //       else this.alertService.failNotification('Parts Manifest Detail', 'Update');
  //     },
  //     error => {
  //       this.alertService.failNotification('Parts Manifest Detail', 'Update');
  //     });
  //   });
  // }

  deleteOrder(event: any, row: Order) {
    if (event) {
      event.preventDefault();
    }
    this.alertService.deleteNotification('Part').then(answer => {
      if (!answer.isConfirmed) return;

      let index = this.partsManifestDetailList.findIndex(e => e.id === row.id); 
      this.partsManifestDetailList.splice(index, 1);
      this.dataSource.data = this.partsManifestDetailList;

      //this.currentPartsManifest.partsManifestDetails = this.currentPartsManifest.partsManifestDetails.filter(e => e.orderId !== row.id);
      this.cd.detectChanges();
    });
  }

  deletePartsManifestDetail(event: any, row: PartsManifestDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.deleteNotification(this.isCreateMode() ? 'Order' : 'Parts Manifest Detail').then(answer => {
      if (!answer.isConfirmed) return;

      if (this.isCreateMode()) {
        // Remove Order from OrderList
        let index = this.partsManifestDetailList.findIndex(e => e.id === row.id); 
        this.partsManifestDetailList.splice(index, 1);
        this.dataSource.data = this.partsManifestDetailList;
      }
      else {
      //   this.partsManifestService.softDeletePartsManifestDetail(row).subscribe(result => {
      //     if (result) {
      //       let tempData = this.dataSource.data;
      //       tempData.splice(this.dataSource.data.findIndex(e => e.id === row.id), 1);
      //       this.dataSource.data = tempData;
      //       this.alertService.successNotification("Parts Manifest Detail", "Delete");
      //       this.cd.detectChanges();
      //     }
      //     else {
      //       this.alertService.failNotification("Parts Manifest Detail", "Delete");
      //     }
      
       //});
      }
    });
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '';
  }

  getTotalStops() {
    this.totalStops = 0;
    let tempCustomer = '';
    this.dataSource.data.forEach(e => {
      if (tempCustomer !== e.shipAddressName) {
        tempCustomer = e.shipAddressName;
        this.totalStops += 1;
      }
    });
  }
}