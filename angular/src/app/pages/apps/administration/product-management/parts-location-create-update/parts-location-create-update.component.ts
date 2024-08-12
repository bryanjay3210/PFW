import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { WarehouseLocation, Product, User, WarehousePartDTO } from 'src/services/interfaces/models';
import { ThisReceiver } from '@angular/compiler';
import { Observable } from 'rxjs';
import { ProductService } from 'src/services/product.service';
import { map, startWith } from 'rxjs/operators';

@Component({
  selector: 'vex-parts-location-create-update',
  templateUrl: './parts-location-create-update.component.html',
  styleUrls: ['./parts-location-create-update.component.scss'],

})
export class PartsLocationCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "https://icons.iconarchive.com/icons/treetog/junior/256/contacts-icon.png";
  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  productId: any;
  warehouseId: any;
  warehouseLocationId: any;
  warehouseStockId: any;

  constructor(
    @Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<PartsLocationCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    if (this.defaults.part) {
      this.mode = 'update';
      this.defaults = this.defaults.part;
      this.productId = this.defaults.productId;
      this.warehouseId = this.defaults.warehouseId;
      this.warehouseLocationId = this.defaults.warehouseLocationId;
      this.warehouseStockId = this.defaults.warehouseStockId;
      
    } 
    else {
      this.mode = 'create';
      this.productId = this.defaults.productId;
      this.warehouseId = 1;
      this.warehouseLocationId = 0;
      this.warehouseStockId = 0;
      this.defaults = {} as WarehousePartDTO;
    }

    this.form = this.fb.group({
      id: [PartsLocationCreateUpdateComponent.id++],
      productId: [this.productId],
      warehouseId: [this.warehouseId],
      location: [this.defaults.location || '', Validators.required],
      zoning: [this.defaults.zoning || '', Validators.required],
      height: [this.defaults.height || '', Validators.required],
      quantity: [this.defaults.quantity || '', Validators.required],
    });
  }

  save() {
    if (this.mode === 'create') {
      if (this.form.valid) {
        this.alertService.createNotification("Parts Location").then(answer => {
          if (!answer.isConfirmed) return;
          this.createPartsLocation();
        });
      }
      else this.alertService.validationNotification("Parts Location");
    }
    else if (this.mode === 'update') {
      if (this.form.valid) {
        this.alertService.updateNotification("Parts Location").then(answer => {
          if (!answer.isConfirmed) return;
          this.updatePartsLocation();
        });
      }
      else this.alertService.validationNotification("Parts Location");
    }
  }

  createPartsLocation() {
    const warehouseLocation = {} as WarehouseLocation;
    this.mapFomValuesToInterface(warehouseLocation);
    this.dialogRef.close(warehouseLocation);
  }

  updatePartsLocation() {
    const warehouseLocation = {} as WarehouseLocation;
    this.mapFomValuesToInterface(warehouseLocation);
    this.dialogRef.close(warehouseLocation);
  }

  mapFomValuesToInterface(warehouseLocation: WarehouseLocation) {
    warehouseLocation.productId = this.productId;
    warehouseLocation.warehouseId = this.warehouseId;
    warehouseLocation.location = this.form.value.location;
    warehouseLocation.zoning = this.form.value.zoning;
    warehouseLocation.height = this.form.value.height;
    warehouseLocation.quantity = this.form.value.quantity;

    warehouseLocation.isActive = true;
    warehouseLocation.isDeleted = false;
    warehouseLocation.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
    warehouseLocation.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;

    if (this.isUpdateMode()) {
      warehouseLocation.modifiedBy = this.currentUser.userName;
      warehouseLocation.modifiedDate = moment(new Date());
      warehouseLocation.id = this.warehouseLocationId;
    }
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
