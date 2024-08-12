import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { ItemMasterlistReference, Product, Role, User } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { ProductService } from 'src/services/product.service';


@Component({
  selector: 'vex-item-masterlist-reference-create-update',
  templateUrl: './item-masterlist-reference-create-update.component.html',
  styleUrls: ['./item-masterlist-reference-create-update.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class ItemMasterlistReferenceCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  inputType = 'password';

  roleList: Role[];
  userTypeList: Lookup[];
  accessTypeList: Lookup[];
 
  productList: Product[];

  visible = false;

  productCtrl: UntypedFormControl;
  filteredProducts$: Observable<Product[]>;
  selectedProduct: Product;

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: ItemMasterlistReference,
    private dialogRef: MatDialogRef<ItemMasterlistReferenceCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private productService: ProductService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ItemMasterlistReference);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.getLookups();

    if (this.defaults) {
      this.mode = 'update';
    } else {
      this.defaults = {} as ItemMasterlistReference;
    }

    this.form = this.fb.group({
      id: [ItemMasterlistReferenceCreateUpdateComponent.id++],
      partNumber: [this.defaults.partNumber || ''],
      partsLinkNumber: [this.defaults.partsLinkNumber || '', Validators.required],
      oemNumber: [this.defaults.oemNumber || '', Validators.required]
    });
  }

  getLookups() {
    this.productService.getProducts().subscribe((result: Product[]) => {
      (this.productList = result);
      this.initializeProductList();
      this.selectedProduct = this.productList.find(p => p.partNumber === this.defaults.partNumber);
      this.productCtrl.setValue(this.defaults.partNumber);
    });
  }

  initializeProductList() {
    this.productCtrl = new UntypedFormControl();
    this.filteredProducts$ = this.productCtrl.valueChanges.pipe(
      startWith(''),
      map(product => product ? this.filterProducts(product) : this.productList.slice())
    );
  }

  filterProducts(name: string): any {
    return this.productList.filter(product =>
      product.partNumber.toLowerCase().indexOf(name.toLowerCase()) === 0);
  }

  onProductSelectionChange(name: string) {
    this.selectedProduct = this.productList.find(c => c.partNumber === name);
    this.cd.detectChanges();
  }

  resetProductControl() {
    this.productCtrl.reset();
    this.selectedProduct = undefined;
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("Item Masterlist Reference").then(answer => {
          if (!answer.isConfirmed) return;
          this.createItemMasterlistReference();
        });
      }
      else this.alertService.validationNotification("Item Masterlist Reference");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("Item Masterlist Reference").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateItemMasterlistReference();
        });
      }
      else this.alertService.validationNotification("Item Masterlist Reference");
    }
  }

  createItemMasterlistReference() {
    const itemMasterlistReference = {} as ItemMasterlistReference;
    this.mapFormValuesToItemMasterlistReference(itemMasterlistReference);
    this.dialogRef.close(itemMasterlistReference);
  }

  updateItemMasterlistReference() {
    const itemMasterlistReference = {} as ItemMasterlistReference;
    this.mapFormValuesToItemMasterlistReference(itemMasterlistReference);
    this.dialogRef.close(itemMasterlistReference);
  }

  mapFormValuesToItemMasterlistReference(itemMasterlistReference: ItemMasterlistReference) {
    itemMasterlistReference.productId = this.selectedProduct.id;
    itemMasterlistReference.partNumber = this.selectedProduct.partNumber;
    itemMasterlistReference.partsLinkNumber = this.form.value.partsLinkNumber;
    itemMasterlistReference.oemNumber = this.form.value.oemNumber;

    itemMasterlistReference.isActive = this.isCreateMode() ? true : this.defaults.isActive;
    itemMasterlistReference.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    itemMasterlistReference.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
    itemMasterlistReference.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;

    if (this.isUpdateMode()) {
      itemMasterlistReference.modifiedBy = this.currentUser.userName;
      itemMasterlistReference.modifiedDate = moment(new Date());
      itemMasterlistReference.id = this.defaults.id;
    }
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
