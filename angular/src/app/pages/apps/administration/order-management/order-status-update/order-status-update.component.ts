import { ChangeDetectorRef, Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { Order, OrderDetail, User } from 'src/services/interfaces/models';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';

@Component({
  selector: 'vex-order-status-create-update',
  templateUrl: './order-status-update.component.html',
  styleUrls: ['./order-status-update.component.scss']
})
export class OrderStatusUpdateComponent implements OnInit {
  @ViewChild('carrier', { static: false }) carrier: ElementRef;
  @ViewChild('product', { static: false }) product: ElementRef;
  @ViewChild('tracking', { static: false }) tracking: ElementRef;

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  orderStatusList =
  [
    { id: 1, code: 'Open' },
    { id: 4, code: 'Delivered'}
  ]

  carrierCtrl = new UntypedFormControl();
  productCtrl = new UntypedFormControl();
  trackingCtrl = new UntypedFormControl();
  
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  currentProduct: any;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Order,
    private router: Router,
    private dialogRef: MatDialogRef<OrderStatusUpdateComponent>,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
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

    this.carrierCtrl.setValue('FedEx');
    this.tracking.nativeElement.focus();
    // this.form = this.fb.group({
    //   id: [OrderStatusUpdateComponent.id++],
    //   // orderStatusId: [this.defaults.orderStatusId || ''],
    //   // trackingNumber: [this.defaults.trackingNumber || ''],
    //   // carrier: [this.defaults.carrier || 'FedEx'],
    //   //quantityShipped: [this.defaults.quantityShipped || '0'],
    // });
  }

  carrierEntered(event: any) {
    if (event) {
      event.stopPropagation();
    }
    if (!this.carrierCtrl.value || this.carrierCtrl.value.trim().length === 0) return;
    this.tracking.nativeElement.focus();
  }

  trackingEntered(event: any) {
    if (event) {
      event.stopPropagation();
    }
    if (!this.trackingCtrl.value || this.trackingCtrl.value.trim().length === 0) return;
    this.product.nativeElement.focus();
  }

  searchProductInList(event: any) {
    if (event) {
      event.stopPropagation();
    }

    if (this.productCtrl.value && this.productCtrl.value.trim().length === 0) return;

    let currentProduct = this.defaults.orderDetails.find(e => e.partNumber.trim().toLowerCase() === this.productCtrl.value.trim().toLowerCase());

    if (!currentProduct) {
      this.alertService.notFoundNotification(this.productCtrl.value);
      this.productCtrl.setValue('');
      return;
    }

    if (currentProduct.orderQuantity === currentProduct.shippedQuantity) {
      this.alertService.errorNotification('Shipped Quantity should not exceed the Order Quantity.');
      this.productCtrl.setValue('');
      return
    }

    currentProduct.carrier = this.carrierCtrl.value;
    currentProduct.shippedQuantity = Number(currentProduct.shippedQuantity) + 1;
    currentProduct.trackingNumber = this.trackingCtrl.value;
    currentProduct.modifiedBy = this.currentUser.userName;
    currentProduct.modifiedDate = moment(new Date());

    if (currentProduct.orderQuantity === currentProduct.shippedQuantity) {
      currentProduct.statusId = 4; //Delivered
    }

    this.productCtrl.setValue('');
  }

  save() {
    if (!this.validOrderStatus()) {
      this.alertService.validationNotification("Order Status");
      return;
    }

    this.alertService.updateNotification("Order Status").then(answer => {
      if (!answer.isConfirmed) return;
      this.updateOrderStatus();
    });

    // if (this.form.valid) {
    //   if (!this.validOrderStatus()) return;
    //   this.alertService.updateNotification("Order Status").then(answer => {
    //     if (!answer.isConfirmed) return;
    //     this.updateOrderStatus();
    //   });
    // }
    // else this.alertService.validationNotification("Order Status");
  }

  updateOrderStatus() {
    let order = this.defaults;
    //order.orderStatusId = this.form.value.orderStatusId;
    //order.orderStatusName = this.orderStatusList.find(e => e.id === order.orderStatusId).code;
    //order.trackingNumber = this.form.value.trackingNumber;
    //order.carrier = this.form.value.carrier;
    //order.quantityShipped = this.form.value.quantityShipped;
    order.modifiedBy = this.currentUser.userName;
    order.modifiedDate = moment(new Date());
    this.dialogRef.close(order);
  }

  shippedChange(row: OrderDetail, event: any) {
    let evnt = event;
    if (evnt.target.value > row.orderQuantity) {
      evnt.target.value = row.orderQuantity
    }
    row.shippedQuantity = event.target.value;
  }

  getValue(row: OrderDetail) {
    if (row.shippedQuantity) {
      return row.shippedQuantity;
    }
    else {
      if (this.defaults.orderStatusId === 4) {
        return row.shippedQuantity;  
      }
      else {
        row.shippedQuantity = 0;
        return row.shippedQuantity;
      }
    }
  }

  validOrderStatus(): boolean {
    let result = true;

    // if (this.carrierCtrl.value)

    // if (this.form.value.trackingNumber === undefined || this.form.value.trackingNumber.length === 0) {
    //   this.alertService.validationRequiredNotification('Tracking Number is required.');
    //   return false;
    // }

    // if (this.form.value.orderStatusId === undefined || this.form.value.orderStatusId !== 4) {
    //   this.alertService.validationRequiredNotification('Delivered Status is required.');
    //   return false;
    // }

    // if (this.form.value.carrier === undefined || this.form.value.carrier.length === 0) {
    //   this.alertService.validationRequiredNotification('Carrier is required.');
    //   return false;
    // }

    // if (this.defaults.orderDetails.filter(d => d.shippedQuantity > 0).length === 0) {
    //   this.alertService.validationRequiredNotification('At least one Part with one Shipped Quantity is required.');
    //   result = false;
    // }

    return result;
  }
}



