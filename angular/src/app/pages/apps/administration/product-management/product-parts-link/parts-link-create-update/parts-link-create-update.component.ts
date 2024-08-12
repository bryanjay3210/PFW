import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { ItemMasterlistReference, Product, User } from 'src/services/interfaces/models';
import { ThisReceiver } from '@angular/compiler';
import { Observable } from 'rxjs';
import { ProductService } from 'src/services/product.service';
import { map, startWith } from 'rxjs/operators';

@Component({
  selector: 'vex-parts-link-create-update',
  templateUrl: './parts-link-create-update.component.html',
  styleUrls: ['./parts-link-create-update.component.scss'],

})
export class PartsLinkCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "https://icons.iconarchive.com/icons/treetog/junior/256/contacts-icon.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  isMainPartsLink: boolean = false;
  isMainOEM: boolean = false;

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(
    @Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<PartsLinkCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private productService: ProductService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    if (this.defaults.data) {
      this.mode = 'update';
      this.defaults = this.defaults.data;
      this.isMainPartsLink = this.defaults.isMainPartsLink;
      this.isMainOEM = this.defaults.isMainOEM;
    } else {
      let productId = this.defaults.productId;
      let productPartNumber = this.defaults.partNumber;
      this.defaults = {} as ItemMasterlistReference;
      this.defaults.productId = productId;
      this.defaults.partNumber = productPartNumber;
    }

    this.form = this.fb.group({
      id: [PartsLinkCreateUpdateComponent.id++],
      partNumber: [this.defaults.partNumber || ''],
      partsLinkNumber: [this.defaults.partsLinkNumber || '', Validators.required],
      oemNumber: [this.defaults.oemNumber || '', Validators.required],
    });
  }

  save() {
    if (this.mode === 'create') {
      if (this.form.valid) {
        this.alertService.createNotification("Parts Link").then(answer => {
          if (!answer.isConfirmed) return;
          this.createPartsLink();
        });
      }
      else this.alertService.validationNotification("Parts Link");
    }
    else if (this.mode === 'update') {
      if (this.form.valid) {
        this.alertService.updateNotification("Parts Link").then(answer => {
          if (!answer.isConfirmed) return;
          this.updatePartsLink();
        });
      }
      else this.alertService.validationNotification("Parts Link");
    }
  }

  createPartsLink() {
    const partsLink = {} as ItemMasterlistReference;
    this.mapFomValuesToInterface(partsLink);
    this.dialogRef.close(partsLink);
  }

  updatePartsLink() {
    const partsLink = {} as ItemMasterlistReference;
    this.mapFomValuesToInterface(partsLink);
    this.dialogRef.close(partsLink);
  }

  mapFomValuesToInterface(partsLink: ItemMasterlistReference) {
    partsLink.productId = this.defaults.productId;
    partsLink.partNumber = this.defaults.partNumber;
    partsLink.partsLinkNumber = this.form.value.partsLinkNumber;
    partsLink.oemNumber = this.form.value.oemNumber;
    partsLink.isMainPartsLink = this.isMainPartsLink;
    partsLink.isMainOEM = this.isMainOEM;

    partsLink.isActive = this.isCreateMode() ? true : this.defaults.isActive;
    partsLink.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    partsLink.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
    partsLink.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;

    if (this.isUpdateMode()) {
      partsLink.modifiedBy = this.currentUser.userName;
      partsLink.modifiedDate = moment(new Date());
      partsLink.id = this.defaults.id;
    }

  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  onMainPartsLinkClick(checked: boolean) {
    if (!checked) {
      this.alertService.warningConfirmationNotification("Unchecking this would remove the Main Parts Link for the product " + this.defaults.partNumber).then(answer => {
        if (!answer.isConfirmed) {
          this.isMainPartsLink = true;
          this.cd.detectChanges();
          return;
        }
        this.isMainPartsLink = checked;
      });
    }
    else this.isMainPartsLink = checked;
  }

  onMainOEMClick(checked: boolean) {
    if (!checked) {
      this.alertService.warningConfirmationNotification("Unchecking this would remove the Main OEM for the product " + this.defaults.partNumber).then(answer => {
        if (!answer.isConfirmed) {
          this.isMainOEM = true;
          this.cd.detectChanges();
          return;
        }
        this.isMainOEM = checked;
      });
    }
    else this.isMainOEM = checked;
  }
}
