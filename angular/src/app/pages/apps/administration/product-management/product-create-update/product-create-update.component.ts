import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { Category, Product, Role, Sequence, User, WarehouseLocation, WarehousePartDTO, WarehouseStock } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { CategorytService } from 'src/services/category.service';
import { SequenceService } from 'src/services/sequence.service';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { LookupService } from 'src/services/lookup.service';
import { PartsLocationCreateUpdateComponent } from '../parts-location-create-update/parts-location-create-update.component';
import { WarehouseLocationService } from 'src/services/warehouselocation.service';


@Component({
  selector: 'vex-product-create-update',
  templateUrl: './product-create-update.component.html',
  styleUrls: ['./product-create-update.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class ProductCreateUpdateComponent implements OnInit {
  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";
  imageNotAvailable = "assets/img/imagenotavailable.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  inputType = 'password';

  roleList: Role[];
  userTypeList: Lookup[];
  accessTypeList: Lookup[];
  categoryList: Category[];
  sequenceList: Sequence[];
  locationList: WarehousePartDTO[];

  statusList = [
    { id: 1, name: 'Active' },
    { id: 2, name: 'Inactive' },
    { id: 3, name: 'For Deletion' },
  ];

  partSizeList = [
    { id: 1, name: 'Small' },
    { id: 2, name: 'Medium' },
    { id: 3, name: 'Large' },
  ];

  selectedTab = 0;
  visible = false;
  categoryCtrl: UntypedFormControl;
  filteredCategories$: Observable<Category[]>;
  sequenceCtrl: UntypedFormControl;
  filteredSequences$: Observable<Sequence[]>;
  selectedCategory: Category;
  selectedSequence: Sequence;
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  toggleActive = new FormControl(true, []);
  toggleAPI = new FormControl(true, []);

  isActive: boolean = true;
  isCAProduct: boolean = false;
  isNVProduct: boolean = false;
  isAPIAllowed: boolean = false;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Product,
    private dialogRef: MatDialogRef<ProductCreateUpdateComponent>,
    private dialog: MatDialog,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private categoryService: CategorytService,
    private warehouseLocationservice: WarehouseLocationService,
    private lookupService: LookupService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ProductManagement);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.getLookups();

    if (this.defaults) {
      this.mode = 'update';
      this.isCAProduct = this.defaults.isCAProduct;
      this.isNVProduct = this.defaults.isNVProduct;
      this.isActive = this.defaults.isActive;
      this.isAPIAllowed = this.defaults.isAPIAllowed;
      this.getWarehouseParts(this.defaults.id);
    } else {
      this.defaults = {} as Product;
      this.isActive = true;
      this.isAPIAllowed = false;
    }

    this.toggleActive = new FormControl(this.isActive, []);
    this.toggleAPI = new FormControl(this.isAPIAllowed, []);

    this.form = this.fb.group({
      id: [ProductCreateUpdateComponent.id++],
      partNumber: [this.defaults.partNumber || '', Validators.required],
      partDescription: [this.defaults.partDescription || '', Validators.required],
      brand: [this.defaults.brand || ''],
      priceLevel1: [this.defaults.priceLevel1 || 0],
      priceLevel2: [this.defaults.priceLevel2 || 0],
      priceLevel3: [this.defaults.priceLevel3 || 0],
      priceLevel4: [this.defaults.priceLevel4 || 0],
      priceLevel5: [this.defaults.priceLevel5 || 0],
      priceLevel6: [this.defaults.priceLevel6 || 0],
      priceLevel7: [this.defaults.priceLevel7 || 0],
      priceLevel8: [this.defaults.priceLevel8 || 0],
      partsLinkNumber: [''],
      oemNumber: [''],
      oemListPrice: [this.defaults.oemListPrice || 0],
      previousCost: [this.defaults.previousCost || 0],
      currentCost: [this.defaults.currentCost || 0],
      imageUrl: [this.defaults.imageUrl || ''],
      dateAdded: [this.defaults.dateAdded || moment(new Date())],
      yearFrom: [this.defaults.yearFrom || 0],
      yearTo: [this.defaults.yearTo || 0],
      categoryId: [this.defaults.categoryId || ''],
      sequenceId: [this.defaults.sequenceId || ''],
      statusId: [this.defaults.statusId || ''],
      partSizeId: [this.defaults.partSizeId || ''],
      onReceivingHold: [this.defaults.onReceivingHold || ''],
      onOrder: [this.defaults.onOrder || ''],
      isDropShipAllowed: [this.defaults.isDropShipAllowed || ''],
      isWebsiteOption: [this.defaults.isWebsiteOption || ''],
      // isActiveProduct: [this.defaults.isActive || ''],
    });

    this.toggleActive.valueChanges.subscribe((newToggleValue:boolean) => {
      this.isActive = newToggleValue;
    });

    this.toggleAPI.valueChanges.subscribe((newToggleValue:boolean) => {
      this.isAPIAllowed = newToggleValue;
    });
  }

  getWarehouseParts(productId: number) {
    this.lookupService.getWarehousePartsByProductId(productId).subscribe(result => {
      if (result) {
        this.locationList = result;
      }
    });
  }

  getLookups() {
    this.categoryService.getCategories().subscribe((result: Category[]) => {
      (this.categoryList = result);
      this.initializeCategoryList();

      if (this.mode === 'update') {
        this.selectedCategory = this.categoryList.find(c => c.id === this.defaults.categoryId);

        if (this.selectedCategory) {
          this.categoryCtrl.setValue(this.selectedCategory.description);
          this.sequenceList = this.selectedCategory.sequences;
          this.initializeSequenceList();
          this.selectedSequence = this.sequenceList.find(s => s.id === this.defaults.sequenceId);
          this.sequenceCtrl.setValue(this.selectedSequence.categoryGroupDescription);
        }
      }
    });
  }

  initializeCategoryList() {
    this.categoryCtrl = new UntypedFormControl();
    this.filteredCategories$ = this.categoryCtrl.valueChanges.pipe(
      startWith(''),
      map(category => category ? this.filterCategories(category) : this.categoryList.slice())
    );
  }

  initializeSequenceList() {
    this.sequenceCtrl = new UntypedFormControl();
    this.filteredSequences$ = this.sequenceCtrl.valueChanges.pipe(
      startWith(''),
      map(sequence => sequence ? this.filterSequences(sequence) : this.sequenceList.slice())
    );
  }

  filterCategories(name: string): any {
    return this.categoryList.filter(category =>
      category.description.toLowerCase().indexOf(name.toLowerCase()) === 0)
  }

  filterSequences(code: string) {
    return this.sequenceList.filter(sequence =>
      sequence.categoryGroupDescription.toLowerCase().indexOf(code.toLowerCase()) === 0);
  }

  onCategorySelectionChange(name: string) {
    this.selectedCategory = this.categoryList.find(c => c.description === name);
    this.sequenceList = this.selectedCategory.sequences;
    this.selectedSequence = undefined;
    this.initializeSequenceList();
    this.cd.detectChanges();
  }

  onSequenceSelectionChange(name: string) {
    this.selectedSequence = undefined;
    this.selectedSequence = this.sequenceList.find(s => s.categoryGroupDescription === name);
  }

  resetCategoryControl() {
    this.categoryCtrl.reset();
    this.selectedCategory = undefined;
    this.sequenceList = [];
    this.selectedSequence = undefined;
    this.initializeSequenceList();
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("Product").then(answer => {
          if (!answer.isConfirmed) return;
          this.createProduct();
        });
      }
      else this.alertService.validationNotification("Product");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("Product").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateProduct();
        });
      }
      else this.alertService.validationNotification("Product");
    }
  }

  createPartLocation() {
    this.dialog.open(PartsLocationCreateUpdateComponent, {
      height: '30%',
      // width: '50%',
      data: { productId: this.defaults.id }
    }).afterClosed().subscribe((warehouseLocation: WarehouseLocation) => {
      if (warehouseLocation) {
        this.warehouseLocationservice.createWarehouseLocationWithStock(warehouseLocation).subscribe((result: WarehousePartDTO[]) => {
          if (result) {
            this.getWarehouseParts(this.defaults.id);
            this.alertService.successNotification("Warehouse Location", "Create");
          }
          else {
            this.alertService.failNotification("Warehouse Location", "Create");
          }
        });
      }
    });
  }

  updatePartLocation(item: WarehousePartDTO) {
    this.dialog.open(PartsLocationCreateUpdateComponent, {
      height: '30%',
      // width: '50%',
      data: { productId: this.defaults.id, part: item }
    }).afterClosed().subscribe((warehouseLocation: WarehouseLocation) => {
      if (warehouseLocation) {
        this.warehouseLocationservice.updateWarehouseLocationWithStock(warehouseLocation).subscribe((result: WarehousePartDTO[]) => {
          if (result) {
            this.getWarehouseParts(this.defaults.id);
            this.alertService.successNotification("Warehouse Location", "Update");
          }
          else {
            this.alertService.failNotification("Warehouse Location", "Update");
          }
        });
      }
    });
  }

  createProduct() {
    const product = {} as Product;
    this.mapFormValuesToProduct(product);
    this.dialogRef.close(product);
  }

  updateProduct() {
    const product = {} as Product;
    this.mapFormValuesToProduct(product);
    this.dialogRef.close(product);
  }

  mapFormValuesToProduct(product: Product) {
    product.partNumber = this.form.value.partNumber;
    product.brand = this.form.value.brand;
    product.partDescription = this.form.value.partDescription;
    product.partSizeId = this.form.value.partSizeId;

    product.priceLevel1 = this.form.value.priceLevel1;
    product.priceLevel2 = this.form.value.priceLevel2;
    product.priceLevel3 = this.form.value.priceLevel3;
    product.priceLevel4 = this.form.value.priceLevel4;
    product.priceLevel5 = this.form.value.priceLevel5;
    product.priceLevel6 = this.form.value.priceLevel6;
    product.priceLevel7 = this.form.value.priceLevel7;
    product.priceLevel8 = this.form.value.priceLevel8;

    product.oemListPrice = this.form.value.oemListPrice;
    product.previousCost = this.form.value.previousCost;
    product.currentCost = this.form.value.currentCost;
    product.imageUrl = this.form.value.imageUrl;
    product.categoryId = this.selectedCategory.id;
    product.sequenceId = this.selectedSequence.id;
    product.statusId = this.form.value.statusId;
    product.dateAdded = moment(this.form.value.dateAdded);

    product.isCAProduct = this.isCAProduct;
    product.isNVProduct = this.isNVProduct;

    if (this.isCreateMode()) {
      product.partsLinkNumber = this.form.value.partsLinkNumber;
      product.oemNumber = this.form.value.oemNumber;
    }

    product.isActive = this.isActive; //this.isCreateMode() ? true : this.defaults.isActive;
    product.isAPIAllowed = this.isAPIAllowed;

    product.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    product.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
    product.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;

    if (this.isUpdateMode()) {
      product.modifiedBy = this.currentUser.userName;
      product.modifiedDate = moment(new Date());
      product.originalPartNumber = this.defaults.originalPartNumber;
      product.id = this.defaults.id;
    }
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  onTabChange(event: MatTabChangeEvent) {
    this.selectedTab = event.index;
  }

  setCAProduct(event: any) {
    this.isCAProduct = event.checked;
  }

  setNVProduct(event: any) {
    this.isNVProduct = event.checked;
  }

  setDefaultImage(event: any) {
    event.src = this.imageNotAvailable;
    this.cd.detectChanges();
  }
}