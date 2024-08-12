import { ChangeDetectorRef, Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { OrderDetail, Product, User, WarehouseLocation } from 'src/services/interfaces/models';
import { ProductService } from 'src/services/product.service';
import { WarehouseLocationService } from 'src/services/warehouselocation.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';

@Component({
  selector: 'vex-rga-inspected-code',
  templateUrl: './rga-inspected-code.component.html',
  styleUrls: ['./rga-inspected-code.component.scss']
})
export class RGAInspectedCodeComponent implements OnInit {
  @ViewChild('state', { static: false }) state: ElementRef;
  @ViewChild('location', { static: false }) location: ElementRef;
  @ViewChild('product', { static: false }) product: ElementRef;

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  stateList =
    [
      { id: 1, code: 'CA' },
      { id: 2, code: 'NV' }
    ]

  rgaInspectedCodeList =
    [
      { id: 1, code: 'Return to Stock' }, //- "Required - Enter Warehouse Location"
      { id: 2, code: 'Damaged - Trash' },
      { id: 3, code: 'Return to Vendor' }, //- Add to Location 77 for CA , Location 88 for NV
      { id: 4, code: 'Mislabel' }, //- "Required Enter new Partnumber" then Enter Warehouse Location"
      { id: 5, code: 'Did not return' }
    ]

  inspectedCodeCtrl = new UntypedFormControl();
  stateCtrl = new UntypedFormControl();
  locationCtrl = new UntypedFormControl();
  productCtrl = new UntypedFormControl();

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  currentProduct: Product;
  currentWarehouseLocation: WarehouseLocation;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: OrderDetail,
    private router: Router,
    private dialogRef: MatDialogRef<RGAInspectedCodeComponent>,
    private warehouseLocationService: WarehouseLocationService,
    private productService: ProductService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.inspectedCodeCtrl.setValue(this.defaults.rgaInspectedCode);
    this.stateCtrl.setValue(this.defaults.rgaState);
    this.locationCtrl.setValue(this.defaults.rgaLocation);
    this.productCtrl.setValue(this.defaults.rgaPartNumber);
  }

  inspectedCodeChanged() {
    this.currentWarehouseLocation = undefined;
    this.stateCtrl.setValue(0);
    this.locationCtrl.setValue('');
    this.productCtrl.setValue('');
  }

  searchLocation(event: any) {
    if (event) {
      event.stopPropagation();
    }

    this.currentWarehouseLocation = undefined;

    if (this.stateCtrl.value === 0 || this.stateCtrl.value === '') {
      this.alertService.validationRequiredNotification('Please select a State prior to adding location.');
      this.locationCtrl.setValue('');
      this.productCtrl.setValue('');
      this.state.nativeElement.focus();
      return;
    }

    this.alertService.showBlockUI('Searching Location...')
    this.warehouseLocationService.getWarehouseLocationByLocation(this.stateCtrl.value, this.locationCtrl.value).subscribe(result => {
      if (result) {
        this.alertService.hideBlockUI();
        this.currentWarehouseLocation = result;
        //this.productCtrl.setValue('');
        this.product.nativeElement.focus();
      }
      else {
        this.alertService.hideBlockUI();
        this.alertService.failNotification('Warehouse Location', 'Search');
        this.locationCtrl.setValue('');
        this.location.nativeElement.focus();
      }
    });
  }

  searchProduct(event: any) {
    if (event) {
      event.stopPropagation();
    }
    this.currentProduct = undefined;
    if (!this.productCtrl.value || this.productCtrl.value === '') return;

    this.alertService.showBlockUI('Searching Product...')
    this.productService.getProductByPartNumber(this.productCtrl.value.trim()).subscribe(result => {
      if (result && result.length > 0) {
        this.alertService.hideBlockUI();
        this.currentProduct = result[0];
      }
      else {
        this.alertService.hideBlockUI();
        this.alertService.failNotification('Product', 'Search');
        this.productCtrl.setValue('');
        this.product.nativeElement.focus();
      }
    });
  }

  getWarehouseLocationAndSave()
  {
    this.alertService.showBlockUI('Searching Location...')
    this.warehouseLocationService.getWarehouseLocationByLocation(this.stateCtrl.value, this.locationCtrl.value).subscribe(result => {
      if (result) {
        this.alertService.hideBlockUI();
        this.alertService.updateNotification("Order Inspected Code").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateOrderInspectedCode();
        });
      }
      else {
        this.alertService.hideBlockUI();
        this.alertService.failNotification('Warehouse Location', 'Search');
        this.locationCtrl.setValue('');
        this.location.nativeElement.focus();
      }
    });
  }

  save() {
    if (!this.validOrderStatus()) return;

    if ((this.inspectedCodeCtrl.value === 1 || this.inspectedCodeCtrl.value === 4) && !this.currentWarehouseLocation) {
      this.getWarehouseLocationAndSave();
    }
    else {
      this.alertService.updateNotification("Order Inspected Code").then(answer => {
        if (!answer.isConfirmed) return;
        this.updateOrderInspectedCode();
      });
    }
  }

  updateOrderInspectedCode() {
    let orderDetail = this.defaults;
    orderDetail.rgaInspectedCode = this.inspectedCodeCtrl.value;
    orderDetail.rgaState = this.stateCtrl.value;
    orderDetail.rgaState = this.stateCtrl.value;
    orderDetail.rgaLocation = this.locationCtrl.value;
    //orderDetail.rgaPartNumber = this.inspectedCodeCtrl.value === 1 ? this.defaults.partNumber : this.productCtrl.value;
    orderDetail.rgaPartNumber = this.productCtrl.value;
    orderDetail.modifiedBy = this.currentUser.userName;
    orderDetail.modifiedDate = moment(new Date());
    this.dialogRef.close(orderDetail);
  }

  validOrderStatus(): boolean {
    if (!this.inspectedCodeCtrl.value || this.inspectedCodeCtrl.value === '') return false;
    if (this.inspectedCodeCtrl.value === 2 || this.inspectedCodeCtrl.value === 3 || this.inspectedCodeCtrl.value === 5) return true;

    if (this.inspectedCodeCtrl.value === 1 || this.inspectedCodeCtrl.value === 4) {

      if (this.stateCtrl.value === 0 || this.stateCtrl.value === '') {
        this.alertService.requiredNotification('State is required.');
        return false;
      }

      if (!this.locationCtrl.value || this.locationCtrl.value === '') {
        this.alertService.requiredNotification('Location is required.');
        return false;
      }

      if (this.inspectedCodeCtrl.value === 4) {
        if (!this.productCtrl.value || this.productCtrl.value === '') {
          this.alertService.requiredNotification('Part Number is required.');
          return false;
        }
      }

      return true;
    }
  }

  stateChange() {
    this.currentWarehouseLocation = undefined;
    this.locationCtrl.setValue('');
    // this.productCtrl.setValue('');
    this.location.nativeElement.focus();
  }
}


