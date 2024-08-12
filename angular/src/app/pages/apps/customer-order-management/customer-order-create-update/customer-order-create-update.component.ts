import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, ValidationErrors, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { AlertService } from 'src/services/alert.service';
import { CustomerService } from 'src/services/customer.service';
import { ModuleCode, PartsFilter, UserPermission } from 'src/static-data/enums/enums';
import { User, Order, CustomerDTO, LocationDTO, ProductDTO, OrderDetail, PaymentTerm, PriceLevel, Warehouse, ProductFilterDTO, MakeDTO, ModelDTO, YearDTO, CategoryDTO, SequenceDTO, Contact, ProductListDTO, PaymentHistoryDTO, Vendor, VendorCatalog, WarehouseStock } from 'src/services/interfaces/models';
import { LocationService } from 'src/services/location.service';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { Observable } from 'rxjs/internal/Observable';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { map, startWith } from 'rxjs';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import moment from 'moment';
import { LookupService } from 'src/services/lookup.service';
import { MatSelectChange } from '@angular/material/select';
import { ZoneService } from 'src/services/zone.service';
import { ContactService } from 'src/services/contact.service';
import { ProductService } from 'src/services/product.service';
import { PaymentService } from 'src/services/payment.service';
import { OrderService } from 'src/services/order.service';
import { VendorService } from 'src/services/vendor.service';
import { LocationListComponent } from '../../administration/order-management/location-list/location-list.component';
import { CustomerListComponent } from '../../administration/order-management/customer-list/customer-list.component';
import { ContactCreateUpdateComponent } from '../../administration/customer-management/customer-contact/contact-create-update/contact-create-update.component';
import { ProductListComponent } from '../customer-product-list/product-list.component';

@UntilDestroy()
@Component({
  selector: 'vex-customer-order-create-update',
  templateUrl: './customer-order-create-update.component.html',
  styleUrls: ['./customer-order-create-update.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class CustomerOrderCreateUpdateComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('inputYear', { static: false }) inputYear: ElementRef;
  @ViewChild('inputMake', { static: false }) inputMake: ElementRef;
  @ViewChild('inputModel', { static: false }) inputModel: ElementRef;
  @ViewChild('multiCategory', { static: false }) multiCategory: ElementRef;
  @ViewChild('multiSequence', { static: false }) multiSequence: ElementRef;

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  currentCustomer = {} as CustomerDTO;
  originalOrder = {} as Order;
  productFilterDTO = {} as ProductFilterDTO;
  customersList: CustomerDTO[];
  orderDetailsList: OrderDetail[] = [];
  paymentTermList: PaymentTerm[] = [];
  priceLevelList: PriceLevel[] = [];
  warehouseList: Warehouse[] = [];
  paymentHistoryList: PaymentHistoryDTO[] = [];

  orderStatusList = //OrderStatus;
    [
      { id: 1, code: 'Open' },
      { id: 2, code: 'Posted' },
      { id: 3, code: 'Void' },
      { id: 4, code: 'Delivered'},
      { id: 5, code: 'Credit Memo'}
    ]

  // Auto-Complete Dropdowns
  yearList: YearDTO[];
  makeList: MakeDTO[];
  makeListAll: MakeDTO[];
  modelList: ModelDTO[];
  modelListAll: ModelDTO[];
  categoryList: CategoryDTO[];
  categoryListAll: CategoryDTO[];
  sequenceList: SequenceDTO[];
  sequenceListAll: SequenceDTO[];
  vendorList: Vendor[] = [];

  yearCtrl: UntypedFormControl;
  makeCtrl: UntypedFormControl;
  modelCtrl: UntypedFormControl;
  categoryCtrl: UntypedFormControl;
  sequenceCtrl: UntypedFormControl;

  filteredYears$: Observable<YearDTO[]>;
  filteredMakes$: Observable<MakeDTO[]>;
  filteredModels$: Observable<ModelDTO[]>;
  filteredCategories$: Observable<CategoryDTO[]>;
  filteredSequences$: Observable<SequenceDTO[]>;

  selectedYear: YearDTO;
  selectedMake: MakeDTO;
  selectedModel: ModelDTO;
  selectedCategory: CategoryDTO;
  selectedSequence: SequenceDTO;
  selectedContact: string;

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  vendorOverride = UserPermission.NoAccess;

  subject$: ReplaySubject<OrderDetail[]> = new ReplaySubject<OrderDetail[]>(1);
  data$: Observable<OrderDetail[]> = this.subject$.asObservable();
  dataSource: MatTableDataSource<OrderDetail> | null;
  selection = new SelectionModel<OrderDetail>(true, []);
  searchCtrl = new UntypedFormControl()

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

  selectedCategoryItems: CategoryDTO[] = [];
  dropdownCategorySettings = {};
  selectedSequenceItems: SequenceDTO[] = [];
  dropdownSequenceSettings = {};

  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];

  isQuote: boolean = false;
  canCreateCreditMemo: boolean = false;
  backOrder: Order = undefined;

  columns: TableColumn<OrderDetail>[] = [
    { label: 'IMAGE', property: 'imageUrl', type: 'image', visible: true },
    { label: 'QTY', property: 'orderQuantity', type: 'number', visible: true },
    { label: 'STOCKS', property: 'stocks', type: 'text', visible: true },
    { label: 'PART NUMBER', property: 'partNumber', type: 'text', visible: true},
    { label: 'Tracking', property: 'warehouseTracking', type: 'text', visible: false },
    { label: 'YEAR', property: 'yearFrom', type: 'number', visible: true },
    { label: 'Y-To', property: 'yearTo', type: 'number', visible: false },
    { label: 'DESCRIPTION', property: 'partDescription', type: 'text', visible: true },
    { label: 'PLINKS', property: 'partsLinks', type: 'text', visible: true },
    { label: 'OEM', property: 'oeMs', type: 'text', visible: false },
    { label: 'LIST PRICE', property: 'listPrice', type: 'number', visible: true },
    { label: 'PRICE', property: 'wholesalePrice', type: 'number', visible: true },
    { label: 'DISCOUNT', property: 'discountedPrice', type: 'number', visible: true },
    { label: 'TOTAL', property: 'totalAmount', type: 'number', visible: true },
    { label: 'Brand', property: 'brand', type: 'text', visible: false },
    { label: 'Main PLink', property: 'mainPartsLinkNumber', type: 'text', visible: false },
    { label: 'Main OEM', property: 'mainOEMNumber', type: 'text', visible: false },
    { label: 'Vendor Cds', property: 'vendorCodes', type: 'text', visible: false },
  ];
  
  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<CustomerOrderCreateUpdateComponent>,
    private dialog: MatDialog,
    private fb: UntypedFormBuilder,
    private customerService: CustomerService,
    private locationservice: LocationService,
    private lookupService: LookupService,
    private zoneService: ZoneService,
    private contactService: ContactService,
    private productService: ProductService,
    private paymentService: PaymentService,
    private orderService: OrderService,
    private vendorService: VendorService,
    private cd: ChangeDetectorRef,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerOrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();
    this.productFilterDTO.year = 0;
    this.productFilterDTO.categoryIds = [];
    this.productFilterDTO.sequenceIds = [];
    this.productFilterDTO.make = '';
    this.productFilterDTO.model = '';
    this.productFilterDTO.state = 0;
    this.selectedCategoryItems = [];
    this.selectedSequenceItems = [];

    this.dropdownCategorySettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'description',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 3,
      allowSearchFilter: true,
      maxHeight: 500
    };

    this.dropdownSequenceSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'categoryGroupDescription',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 3,
      allowSearchFilter: true,
      maxHeight: 500
    };

    this.getLookups();

    if (this.defaults.customer) {
      
      this.currentCustomer = this.defaults.customer;
      this.initializeFormGroup();
      this.updateCustomerDetails();
      this.handleSummaryOnChangeEvent();
      this.cd.detectChanges();
    }
    else {
      this.alertService.failNotification('Customer', 'Fetch');
      this.dialogRef.close();
    }

    if (this.defaults.order !== undefined) {
      this.defaults = this.defaults.order;
      this.isQuote = this.defaults.isQuote;
      this.mode = 'update';

      if (this.isQuote) {
        this.columns.push({ label: 'Actions', property: 'actions', type: 'button', visible: true });
      }

      this.todayDate = moment(this.defaults.deliveryDate).toDate();
      this.originalOrder = this.defaults;
      this.orderDetailsList = this.defaults.orderDetails;
      this.dataSource.data = this.orderDetailsList;

      this.summaryDiscount = this.defaults.discount;
      this.summaryTax = this.defaults.totalTax;
      this.summaryTaxRate = this.defaults.taxRate;
      this.summarySubTotal = this.defaults.subTotalAmount;
      this.summaryTotal = this.defaults.totalAmount;
      
      this.selectedContact = this.defaults.orderedBy;
      this.initializeFormGroup();
      this.getPaymentHistory();
    } 
    else {
      this.mode = 'create';
      this.defaults = {} as Order;
      this.columns.push({ label: 'Actions', property: 'actions', type: 'button', visible: true });
      this.initializeFormGroup();
    }

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
      id: [CustomerOrderCreateUpdateComponent.id++],
      orderNumber: [this.defaults.orderNumber || '0'],
      quoteNumber: [this.defaults.quoteNumber || '0'],
      accountNumber: [this.defaults.accountNumber || this.currentCustomer.accountNumber],
      customerName: [this.defaults.customerName || this.currentCustomer.customerName],
      phoneNumber: [this.defaults.phoneNumber || ''],
      contactName: [this.currentCustomer.contactName || ''],
      paymentTermId: [this.defaults.paymentTermId || this.currentCustomer.paymentTermId, Validators.required],
      priceLevelId: [this.defaults.priceLevelId || this.currentCustomer.priceLevelId, Validators.required],
      discount: [this.defaults.discount || this.currentCustomer.discount, Validators.required],

      billAddress: [this.defaults.billAddress || '', Validators.required],
      billCity: [this.defaults.billCity || '', Validators.required],
      billState: [this.defaults.billState || '', Validators.required],
      billZipCode: [this.defaults.billZipCode || '', Validators.required],
      billPhoneNumber: [this.defaults.billPhoneNumber || ''],
      billContactName: [this.defaults.billContactName || ''],
      billZone: [this.defaults.billZone || ''],

      shipAddressName: [this.defaults.shipAddressName || '', Validators.required],
      shipAddress: [this.defaults.shipAddress || '', Validators.required],
      shipCity: [this.defaults.shipCity || '', Validators.required],
      shipState: [this.defaults.shipState || '', Validators.required],
      shipZipCode: [this.defaults.shipZipCode || '', Validators.required],
      shipPhoneNumber: [this.defaults.shipPhoneNumber || ''],
      shipContactName: [this.defaults.shipContactName || ''],
      shipZone: [this.defaults.shipZone || ''],

      partNumberFilter: [''],
      customerFilter: [''],
      year: [this.productFilterDTO.year || 0],
      make: [this.productFilterDTO.make || ''],
      model: [this.productFilterDTO.model || ''],

      summaryDiscount: [this.defaults.discount || this.currentCustomer.discount],
      summaryDiscountAmount: [this.getDiscountAmount() || 0.00],
      summarySubTotal: [this.defaults.subTotalAmount ? this.defaults.subTotalAmount.toFixed(2) : 0.00 || 0.00],
      summaryTax: [this.defaults.totalTax ? this.defaults.totalTax.toFixed(2) : "0.00" || "0.00"],
      summaryTaxRate: [this.defaults.taxRate || this.currentCustomer.taxRate],
      summaryTotal: [this.defaults.totalAmount ? this.defaults.totalAmount.toFixed(2) : 0.00 || 0.00],

      purchaseOrderNumber: [this.defaults.purchaseOrderNumber || ''],
      orderedBy: [this.defaults.orderedBy || ''],
      orderedByEmail: [this.defaults.orderedByEmail || ''],
      orderedByPhoneNumber: [this.defaults.orderedByPhoneNumber || ''],
      orderedByNotes: [this.defaults.orderedByNotes || ''],
      deliveryType: [this.defaults.deliveryType || 0],
      deliveryDate: [(this.defaults.deliveryDate && this.defaults.deliveryDate.toString().includes('0001-01-01')) ? '' : this.defaults.deliveryDate || ''],
      deliveryRoute: [this.defaults.deliveryRoute || 0],

      categoryControl: [''],
      sequenceControl: [''],

      invoiceNumber: [this.defaults.invoiceNumber || ''],
      // warehouseTracking: ['']
    });
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  onYearEnter(event) {
    if (event.value && event.value.length > 0) {
      let year = undefined;
      let yearList = this.yearList.filter(e => e.year.includes(event.value));
      if (yearList.length === 1) {
        year = yearList[0];
      }

      if (year) {
        this.yearCtrl.setValue(year.year);
        this.onYearSelectionChange(year.year);
      }
      else {
        this.inputYear.nativeElement.value = '';
        this.yearCtrl.setValue(undefined);
      }
    }
    this.inputMake.nativeElement.focus();
  }

  onMakeEnter(event) {
    if (event.value && event.value.length > 0) {
      let make = undefined;
      let makeList = this.makeList.filter(e => e.make.toLowerCase().trim().includes(event.value.toLowerCase().trim()));
      if (makeList.length === 1) {
        make = makeList[0];
      }

      if (make) {
        this.makeCtrl.setValue(make.make);
        this.onMakeSelectionChange(make.make);
      }
      else {
        this.inputMake.nativeElement.value = '';
        this.makeCtrl.setValue(undefined);
      }
    }
    this.inputModel.nativeElement.focus();
  }

  onModelEnter(event) {
    if (event.value && event.value.length > 0) {
      let model = undefined; 
      let modelList = this.modelList.filter(e => e.model.toLowerCase().trim().includes(event.value.toLowerCase().trim())); 
      if (modelList.length === 1) {
        model = modelList[0]; 
      }

      if (model) {
        this.modelCtrl.setValue(model.model);
        this.onModelSelectionChange(model.model);
      }
      else {
        this.inputModel.nativeElement.value = '';
        this.modelCtrl.setValue(undefined);
      }
    }
  }

  onCategoryEnter(event) {
    if (event.value && event.value.length > 0) {
      let cat = this.categoryList.find(e => e.description.toLowerCase().trim() === event.value.toLowerCase().trim());
      if (cat) {
        this.selectedCategoryItems.push(cat);
        this.modelCtrl.setValue(event.value);
        //this.optionModel.close;
      }
      else {
        this.form.value.categoryControl = '';
      }
    }

    this.multiSequence.nativeElement.focus();
  }

  onSequenceEnter(event) {
  }

  getDiscountAmount(): any {
    let discountAmount = this.defaults.subTotalAmount * (this.defaults.discount / 100);
    return discountAmount.toFixed(2);
  }

  getLookups() {
    this.vendorService.getVendors().subscribe(result => {
      if (result) {
        this.vendorList = result;
      }
    });

    this.lookupService.getYearsListDistinct().subscribe((result: YearDTO[]) => {
      (this.yearList = result);
      this.initializeYearList();
    });

    this.lookupService.getMakesListDistinct().subscribe((result: MakeDTO[]) => {
      (this.makeList = result);
      this.makeListAll = result;
      this.initializeMakeList();
    });

    this.lookupService.getModelsListDistinct().subscribe((result: ModelDTO[]) => {
      (this.modelList = result);
      this.modelListAll = result;
      this.initializeModelList();
    });

    this.lookupService.getCategoriesListDistinct().subscribe((result: CategoryDTO[]) => {
      (this.categoryList = result)
      this.categoryListAll = result;
      this.initializeCategoryList();
    });

    this.lookupService.getSequencesListDistinct().subscribe((result: SequenceDTO[]) => {
      (this.sequenceList = result)
      this.sequenceListAll = result;
      this.initializeSequenceList();
    });

    this.lookupService.getPriceLevels().subscribe((result: PriceLevel[]) => {
      if (result) {
        this.priceLevelList = result;
      }
    });

    this.lookupService.getPaymentTerms().subscribe((result: PaymentTerm[]) => {
      if (result) {
        this.paymentTermList = result;
      }
    });

    this.lookupService.getPriceLevels().subscribe((result: PriceLevel[]) => {
      if (result) {
        this.priceLevelList = result;
      }
    });

    this.lookupService.getWarehouses().subscribe((result: Warehouse[]) => {
      if (result) {
        this.warehouseList = result;
      }
    });
  }

  customFilter = function (countries: any[], query: string): any[] {
    return countries.filter(x => x.name.toLowerCase().contains(query.toLowerCase()));
  };

  initializeYearList() {
    this.yearCtrl = new UntypedFormControl();
    this.filteredYears$ = this.yearCtrl.valueChanges.pipe(
      startWith(''),
      map(year => year ? this.filterYears(year) : this.yearList.slice())
    );
  }

  initializeMakeList() {
    this.makeCtrl = new UntypedFormControl();
    this.filteredMakes$ = this.makeCtrl.valueChanges.pipe(
      startWith(''),
      map(make => make ? this.filterMakes(make) : this.makeList.slice())
    );
  }

  initializeModelList() {
    this.modelCtrl = new UntypedFormControl();
    this.filteredModels$ = this.modelCtrl.valueChanges.pipe(
      startWith(''),
      map(model => model ? this.filterModels(model) : this.modelList.slice())
    );
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

  filterYears(syear: string): any {
    return this.yearList.filter(year => year.year.toLowerCase().includes(syear.toLowerCase()));
  }

  filterMakes(name: string): any {
    return this.makeList.filter(make => make.make.toLowerCase().includes(name.toLowerCase()));
  }

  filterModels(name: string): any {
    return this.modelList.filter(model => model.model.toLowerCase().includes(name.toLowerCase()));
  }

  filterCategories(name: string): any {
    return this.categoryList.filter(category =>
      category.description.toLowerCase().indexOf(name.toLowerCase()) === 0)
  }

  filterSequences(code: string) {
    return this.sequenceList.filter(sequence =>
      sequence.categoryGroupDescription.toLowerCase().indexOf(code.toLowerCase()) === 0);
  }

  onYearSelectionChange(year: string) {
    this.selectedYear = this.yearList.find(c => c.year === year);
    this.productFilterDTO.year = this.selectedYear.yearNumber;
    this.initializeFilters(PartsFilter.Year);
  }

  onMakeSelectionChange(name: string) {
    this.selectedMake = this.makeList.find(c => c.make === name);
    this.productFilterDTO.make = this.selectedMake.make;
    this.initializeFilters(PartsFilter.Make);
  }

  onModelSelectionChange(name: string) {
    this.selectedModel = this.modelList.find(c => c.model === name);
    this.productFilterDTO.model = this.selectedModel.model;
    this.initializeFilters(PartsFilter.Model);
  }

  onCategoryItemSelect(item: any) {
    this.productFilterDTO.categoryIds.push(item.id);
    this.productFilterDTO.sequenceIds = [];
    this.filterSequencesDropdown();
  }

  onCategoryItemDeSelect(item: any) {
    this.productFilterDTO.categoryIds = this.productFilterDTO.categoryIds.filter(e => e !== item.id);
    this.productFilterDTO.sequenceIds = [];
    this.filterSequencesDropdown();
  }

  onCategorySelectDeSelectAll() {
    this.initializeFilters(PartsFilter.Category);
  }

  onSequenceItemSelect(item: any) {
    this.productFilterDTO.sequenceIds.push(item.id)
  }

  onSequenceItemDeSelect(item: any) {
    this.productFilterDTO.sequenceIds = this.productFilterDTO.sequenceIds.filter(e => e !== item.id);
  }

  onSequenceSelectDeSelectAll() {
    this.initializeFilters(PartsFilter.Sequence);
  }

  initializeFilters(filter: PartsFilter) {
    switch (filter) {
      case PartsFilter.Year:
        this.productFilterDTO.make = '';
        this.productFilterDTO.model = '';
        this.productFilterDTO.categoryIds = [];
        this.productFilterDTO.sequenceIds = [];
        this.selectedCategoryItems = [];
        this.selectedSequenceItems = [];
        this.filterMakesDropdown();
        this.filterModelsDropdown();
        this.filterCategoriesDropdown();
        this.filterSequencesDropdown();
        break;
      case PartsFilter.Make:
        this.productFilterDTO.model = '';
        this.productFilterDTO.categoryIds = [];
        this.productFilterDTO.sequenceIds = [];
        this.selectedCategoryItems = [];
        this.selectedSequenceItems = [];
        this.filterModelsDropdown();
        this.filterCategoriesDropdown();
        this.filterSequencesDropdown();
        break;
      case PartsFilter.Model:
        this.productFilterDTO.categoryIds = [];
        this.productFilterDTO.sequenceIds = [];
        this.selectedCategoryItems = [];
        this.selectedSequenceItems = [];
        this.filterCategoriesDropdown();
        this.filterSequencesDropdown();
        break;
      case PartsFilter.Category:
        this.productFilterDTO.categoryIds = [];
        this.productFilterDTO.sequenceIds = [];
        this.selectedCategoryItems = [];
        this.selectedSequenceItems = [];
        this.filterSequencesDropdown();
        break;
      case PartsFilter.Sequence:
        this.productFilterDTO.sequenceIds = [];
        this.selectedSequenceItems = [];
        break;
    }
  }

  private filterMakesDropdown() {
    this.lookupService.getMakesListByYear(this.productFilterDTO).subscribe(result => {
      if (result) {
        this.makeList = result;
        this.initializeMakeList();
      }
    });
  }

  private filterModelsDropdown() {
    this.lookupService.getModelsListByMake(this.productFilterDTO).subscribe(result => {
      if (result) {
        this.modelList = result;
        this.initializeModelList();
      }
    });
  }

  private filterCategoriesDropdown() {
    this.lookupService.getCategoriesListByModel(this.productFilterDTO).subscribe(result => {
      if (result) {
        this.categoryList = result;
        this.initializeCategoryList();
      }
    });
  }

  private filterSequencesDropdown() {
    this.lookupService.getSequencesListByCategoryId(this.productFilterDTO).subscribe(result => {
      if (result) {
        this.sequenceList = result;
        this.initializeSequenceList();
      }
    });
  }

  resetYearControl() {
    this.yearCtrl.reset();
    this.makeCtrl.reset();
    this.modelCtrl.reset();
    this.resetFilterDropdowns('Year');
  }

  resetMakeControl() {
    this.makeCtrl.reset();
    this.modelCtrl.reset();
    this.resetFilterDropdowns('Make');
  }

  resetModelControl() {
    this.modelCtrl.reset();
    this.resetFilterDropdowns('Model');
  }

  resetCategoryControl() {
    this.resetFilterDropdowns('Category');
  }

  resetSequenceControl() {
    this.resetFilterDropdowns('Sequence');
  }

  resetFilterDropdowns(filter: string) {
    switch (filter) {
      case 'Sequence':
        this.productFilterDTO.sequenceIds = [];
        this.selectedSequenceItems = [];
        this.selectedSequence = undefined;
        break;
      case 'Category':
        this.productFilterDTO.categoryIds = [];
        this.productFilterDTO.sequenceIds = [];
        this.selectedCategoryItems = [];
        this.selectedSequenceItems = [];
        this.selectedCategory = undefined;
        this.selectedSequence = undefined;
        this.filterSequencesDropdown();
        break;
      case 'Model':
        this.productFilterDTO.model = '';
        this.productFilterDTO.categoryIds = [];
        this.productFilterDTO.sequenceIds = [];
        this.selectedCategoryItems = [];
        this.selectedSequenceItems = [];
        this.selectedModel = undefined;
        this.selectedCategory = undefined;
        this.selectedSequence = undefined;
        this.filterCategoriesDropdown();
        this.filterSequencesDropdown();
        break;
      case 'Make':
        this.productFilterDTO.make = '';
        this.productFilterDTO.model = '';
        this.productFilterDTO.categoryIds = [];
        this.productFilterDTO.sequenceIds = [];
        this.selectedCategoryItems = [];
        this.selectedSequenceItems = [];
        this.selectedMake = undefined;
        this.selectedModel = undefined;
        this.selectedCategory = undefined;
        this.selectedSequence = undefined;
        this.filterModelsDropdown();
        this.filterCategoriesDropdown();
        this.filterSequencesDropdown();
        break;
      case 'Year':
        this.productFilterDTO.year = 0;
        this.productFilterDTO.make = '';
        this.productFilterDTO.model = '';
        this.productFilterDTO.categoryIds = [];
        this.productFilterDTO.sequenceIds = [];
        this.selectedCategoryItems = [];
        this.selectedSequenceItems = [];
        this.selectedYear = undefined;
        this.selectedMake = undefined;
        this.selectedModel = undefined;
        this.selectedCategory = undefined;
        this.selectedSequence = undefined;
        this.filterModelsDropdown();
        this.filterModelsDropdown();
        this.filterCategoriesDropdown();
        this.filterSequencesDropdown();
        break;
    }
  }

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

  save(mode: string) {
    // if (mode === 'Order') {
    //   if (this.isCreateMode()) {
    //     if (this.form.valid) {
    //       if (this.validOrderLineItems()) {
    //         this.alertService.createNotification("Order").then(answer => {
    //           if (!answer.isConfirmed) { return; }
    //           this.createOrder();
    //         });
    //       }
    //     }
    //     else {
    //       this.getFormValidationErrors();
    //       this.alertService.validationNotification("Order");
    //     }
    //   }
    //   else if (this.isUpdateMode()) {
    //     if (this.form.valid) {
    //       if (this.validOrderLineItems()) {
    //         this.alertService.updateNotification("Order").then(answer => {
    //           if (!answer.isConfirmed) { return; }
    //           this.updateOrder();
    //         });
    //       }
    //     }
    //     else {
    //       this.getFormValidationErrors();
    //       this.alertService.validationNotification("Order");
    //     }
    //   }
    // }

    // else 
    if (mode === 'Quote') {
      if (this.isCreateMode()) {
        if (this.form.valid) {
          if (this.validQuoteLineItems()) {
            this.alertService.createNotification("Order").then(answer => {
              if (!answer.isConfirmed) { return; }
              this.createQuote();
            });
          }
        }
        else {
          this.getFormValidationErrors();
          this.alertService.validationNotification("Order");
        }
      }
      else if (this.isUpdateMode()) {
        if (this.form.valid) {
          if (this.validQuoteLineItems()) {
            this.alertService.updateNotification("Order").then(answer => {
              if (!answer.isConfirmed) { return; }
              this.updateOrder();
            });
          }
        }
        else {
          this.getFormValidationErrors();
          this.alertService.validationNotification("Order");
        }
      }
    }

    // else { // Convert Quote to Order
    //   if (this.form.valid) {
    //     if (this.validOrderLineItems()) {
    //       this.alertService.updateNotification("Quote to Order").then(answer => {
    //         if (!answer.isConfirmed) { return; }
    //         this.convertQuoteToOrder();
    //       });
    //     }
    //   }
    //   else {
    //     this.getFormValidationErrors();
    //     this.alertService.validationNotification("Quote to Order");
    //   }
    // }
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

  validOrderLineItems(): boolean {
    let result = true;
    let title = 'Order Line Item Validation';
    let message = '';

    // Create Order Line Item Validation
    if (this.dataSource.data.length === 0) {
      message = 'Order should have at least 1 part order line item.'
      this.alertService.validationFailedNotification(title, message);
      return false;
    }

    // Stock Quantity & Vendor Code Validation
    for (const e of this.dataSource.data.filter(d => d.onHandQuantity < 1 && d.vendorCode === '')) {
      message = 'Order Part Number ' + e.partNumber + ' has 0 stocks, please select a Vendor Code.'
      this.alertService.validationFailedNotification(title, message);
      result = false;
      break;
    }

    // Order Quantity & Vendor Code Validation
    for (const e of this.dataSource.data.filter(d => d.onHandQuantity < 1 && d.vendorCode !== '' && d.orderQuantity > d.vendorOnHand)) {
      message = 'Order Part Number ' + e.partNumber + ' has order quantity greater than the Vendor quantity, please select another Vendor Code with higher quantity or reduce Order quantity.'
      this.alertService.validationFailedNotification(title, message);
      result = false;
      break;
    }

    // Order Quantity, Stock Quantity & Vendor Code Validation
    for (const e of this.dataSource.data.filter(d => d.onHandQuantity > 0 && d.vendorCode === '' && d.orderQuantity > d.onHandQuantity)) {
      message = 'Order Part Number ' + e.partNumber + ' has Order quantity greater than the Stock quantity, please select a Vendor Code.'
      this.alertService.validationFailedNotification(title, message);
      result = false;
      break;
    }

    // Order Quantity, Stock Quantity & Vendor Code Validation Total
    for (const e of this.dataSource.data.filter(d => d.onHandQuantity > 0 && d.vendorCode !== '' && d.orderQuantity > (d.onHandQuantity + d.vendorOnHand))) {
      message = 'Order Part Number ' + e.partNumber + ' has Order quantity greater than the Stock quantity plus Vendor quantity, please select another Vendor Code with higher quantity or reduce Order quantity.'
      this.alertService.validationFailedNotification(title, message);
      result = false;
      break;
    }

    // Validate List Price Equals Zero
    for (const e of this.dataSource.data.filter(d => d.listPrice === 0)) {
      message = 'Order Part Number ' + e.partNumber + ' has List price equals to Zero, please change the price to at least $1.00.'
      this.alertService.validationFailedNotification(title, message);
      result = false;
      break;
    }

    // Validate Wholesale Price Equals Zero
    for (const e of this.dataSource.data.filter(d => Number(d.wholesalePrice) === 0 && (d.statusId === undefined || Number(d.statusId) === 0))) {
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

    // Validate Purchase Order Number Required
    if (this.form.value.purchaseOrderNumber === undefined || this.form.value.purchaseOrderNumber.length === 0) {
      this.alertService.validationRequiredNotification('Purchase Order Number is required.');
      return false;
    }

    // Validate Ordered By, Email and Phone Number
    if (this.form.value.orderedBy === undefined || this.form.value.orderedBy.length === 0) {
      this.alertService.validationRequiredNotification('Ordered By details are required.');
      return false;
    }

    // Validate Delivery Type Required
    if (this.form.value.deliveryType === undefined || this.form.value.deliveryType === 0) {
      this.alertService.validationRequiredNotification('Order Type is required.');
      return false;
    }

    // Validate Delivery Date Required
    // if (this.form.value.deliveryDate === undefined || this.form.value.deliveryDate === '' || this.defaults.deliveryDate.toString() === '0001-01-01T08:00:00+08:00') {
    if (this.form.value.deliveryDate === undefined || this.form.value.deliveryDate === '') {
      this.alertService.validationRequiredNotification('Delivery Date is required.');
      return false;
    }

    // Validate Delivery Route Required
    if (this.form.value.deliveryRoute === undefined || this.form.value.deliveryRoute === '' || this.form.value.deliveryRoute === 0) {
      this.alertService.validationRequiredNotification('Delivery Route is required.');
      return false;
    }

    return result;
  }

  validQuoteLineItems(): boolean {
    let result = true;
    let title = 'Order Line Item Validation';
    let message = '';

    // Create Quote Line Item Validation
    if (this.dataSource.data.length === 0) {
      message = 'Order should have at least 1 part order line item.'
      this.alertService.validationFailedNotification(title, message);
      return false;
    }

    // Validate Purchase Order Number Required
    if (this.form.value.purchaseOrderNumber === undefined || this.form.value.purchaseOrderNumber.length === 0) {
      this.alertService.validationRequiredNotification('Purchase Order Number is required.');
      return false;
    }
    
    // Validate Ordered By, Email and Phone Number
    if (this.form.value.orderedBy === undefined || this.form.value.orderedBy.length === 0) {
      this.alertService.validationRequiredNotification('Ordered By details are required.');
      return false;
    }

    // Stock Quantity & Vendor Code Validation
    for (const e of this.dataSource.data.filter(d => d.onHandQuantity < 1 && d.vendorCode === '')) {
      message = 'Order Part Number ' + e.partNumber + ' has 0 stocks, please remove item from order stack.'
      this.alertService.validationFailedNotification(title, message);
      result = false;
      break;
    }

    return result;
  }

  createQuote() {
    const order = {} as Order;
    this.mapFormValuesToOrder(order, this.dataSource.data);
    order.isQuote = true;
    this.dialogRef.close(order);
  }

  createOrder() {
    const order = {} as Order;
    this.mapFormValuesToOrder(order, this.dataSource.data);
    order.isQuote = false;
    this.dialogRef.close(order);
  }

  updateOrder() {
    const order = this.defaults;
    this.mapFormValuesToOrder(order, this.dataSource.data);
    order.isQuote = this.defaults.isQuote;
    order.quoteNumber = this.defaults.quoteNumber;
    this.dialogRef.close({ updatedOrder: order, isConvert: false, backOrder: this.backOrder });
  }

  updateOrderSummary() {
    const order = this.defaults;
    this.mapFormValuesToOrder(order, this.dataSource.data);
    order.isQuote = this.defaults.isQuote;
    order.quoteNumber = this.defaults.quoteNumber;
    this.orderService.updateOrderSummary(order).subscribe((result: boolean) => {
      if (result) {
        this.alertService.successNotification(order.isQuote ? "Quote" : "Order", "Update");
      }
      else this.alertService.failNotification(order.isQuote ? "Quote" : "Order", "Update");
    });
  }

  convertQuoteToOrder() {
    const order = {} as Order;
    this.mapFormValuesToOrder(order, this.dataSource.data);
    order.orderDate = moment(new Date());
    order.isQuote = false;
    order.quoteNumber = this.defaults.quoteNumber;
    this.dialogRef.close({ updatedOrder: order, isConvert: true });
  }

  mapFormValuesToOrder(order: Order, dataSource: OrderDetail[]) {
    order.quantity = this.form.value.quantity;
    order.accountNumber = this.form.value.accountNumber;
    order.customerId = this.currentCustomer.id;
    order.customerName = this.currentCustomer.customerName;
    order.phoneNumber = this.currentCustomer.phoneNumber;
    order.priceLevelId = this.currentCustomer.priceLevelId;
    order.paymentTermId = this.currentCustomer.paymentTermId;
    order.discount = this.currentCustomer.discount;
    //order.priceLevelId = this.form.value.priceLevelId;
    order.paymentTermId = this.form.value.paymentTermId;
    order.orderStatusId = this.isCreateMode() ? 1 : this.defaults.orderStatusId; //this.form.value.orderStatusId;
    order.warehouseId = this.currentCustomer.state == 'CA' ? 1 : 2; //this.form.value.warehouseId;
    order.priceLevelName = this.priceLevelList.find(e => e.id === order.priceLevelId).levelName;
    order.paymentTermName = this.paymentTermList.find(e => e.id === order.paymentTermId).termName;
    order.orderStatusName = this.orderStatusList.find(e => e.id === order.orderStatusId).code;
    order.warehouseName = this.warehouseList.find(e => e.id === order.warehouseId).warehouseName;
    order.user = this.currentUser.userName;
    order.billAddress = this.form.value.billAddress;
    order.billCity = this.form.value.billCity;
    order.billContactName = this.form.value.billContactName;
    order.billPhoneNumber = this.form.value.billPhoneNumber;
    order.billState = this.form.value.billState;
    order.billZipCode = this.form.value.billZipCode;
    order.billZone = this.form.value.billZone;
    order.shipAddressName = this.form.value.shipAddressName;
    order.shipAddress = this.form.value.shipAddress;
    order.shipCity = this.form.value.shipCity;
    order.shipContactName = this.form.value.shipContactName;
    order.shipPhoneNumber = this.form.value.shipPhoneNumber;
    order.shipState = this.form.value.shipState;
    order.shipZipCode = this.form.value.shipZipCode;
    order.shipZone = this.form.value.shipZone;
    order.orderDetails = this.mapProductsToOrderDetails(dataSource);
    order.discount = this.form.value.summaryDiscount;
    order.taxRate = this.form.value.summaryTaxRate;
    order.totalTax = this.form.value.summaryTax;
    order.subTotalAmount = this.form.value.summarySubTotal;
    order.totalAmount = this.form.value.summaryTotal;
    order.amountPaid = this.isCreateMode() ? 0 : this.defaults.amountPaid;
    order.balance = this.isCreateMode() ? order.totalAmount : order.totalAmount - this.defaults.amountPaid;
    order.purchaseOrderNumber = this.form.value.purchaseOrderNumber;
    order.orderedBy = this.form.value.orderedBy;
    order.orderedByEmail = this.form.value.orderedByEmail;
    order.orderedByPhoneNumber = this.form.value.orderedByPhoneNumber;
    order.orderedByNotes = this.form.value.orderedByNotes;

    order.deliveryType = this.form.value.deliveryType;
    order.deliveryDate = this.form.value.deliveryDate === '' ? moment(new Date('0001-01-01T00:00:00Z')) : moment(new Date(this.form.value.deliveryDate));

    if (this.form.value.deliveryRoute !== '') {
      order.deliveryRoute = this.form.value.deliveryRoute;
    }

    order.orderDate = this.isCreateMode() ? moment(new Date()) : this.defaults.orderDate;
    order.isActive = this.isCreateMode() ? true : this.defaults.isActive;
    order.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    order.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
    order.createdDate = this.isCreateMode() ? order.orderDate : this.defaults.createdDate;
    order.isCustomerOrder = this.isCreateMode() ? true : this.defaults.isCustomerOrder;

    order.currentCost = order.orderDetails.map(e => e.unitCost).reduce(function (a, b) { return a + b });

    if (this.isUpdateMode()) {
      order.orderNumber = this.defaults.orderNumber;
      order.invoiceNumber = this.defaults.invoiceNumber;
      order.modifiedBy = this.currentUser.userName;
      order.modifiedDate = moment(new Date());
      order.id = this.defaults.id;
    }
  }

  mapProductsToOrderDetails(dataSource: OrderDetail[]): OrderDetail[] {
    const result: OrderDetail[] = [];
    let defaultOrderDetail = {} as OrderDetail;
    dataSource.forEach(element => {
      
      if (this.isUpdateMode()) {
        defaultOrderDetail = this.defaults.orderDetails.find(e => e.partNumber === element.partNumber);
      } 

      const orderDetail = this.isCreateMode() ? {} as OrderDetail : defaultOrderDetail ? defaultOrderDetail : {} as OrderDetail;
      orderDetail.orderId = this.isCreateMode() ? 0 : this.defaults.id;
      orderDetail.productId = element.productId;
      orderDetail.orderQuantity = element.orderQuantity;
      orderDetail.location = element.location;
      orderDetail.warehouseLocationId = element.warehouseLocationId;
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
      orderDetail.discountedPrice = element.discountedPrice;
      orderDetail.totalAmount = element.totalAmount;

      orderDetail.partSize = element.partSize;
      orderDetail.categoryId = element.categoryId;

      orderDetail.partsLinks = element.partsLinks;
      orderDetail.oeMs = element.oeMs;
      orderDetail.vendorCodes = element.vendorCodes;

      orderDetail.isActive = this.isCreateMode() ? true : this.defaults.isActive;
      orderDetail.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
      orderDetail.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
      orderDetail.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;

      orderDetail.price = element.price;
      orderDetail.unitCost = this.getUnitCostTotal(element);
      orderDetail.warehouseTracking = element.warehouseTracking;

      if (this.isUpdateMode()) {
        orderDetail.stocks = element.stocks;
        orderDetail.statusId = element.statusId;
        orderDetail.modifiedBy = this.currentUser.userName;
        orderDetail.modifiedDate = moment(new Date());
      }

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
    return result;
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  openCustomerList() {
    this.dialog.open(CustomerListComponent, {
      height: '100%',
      width: '1000%',
      data: { customerFilter: this.form.value.customerFilter }
    }).afterClosed().subscribe((customer: CustomerDTO) => {
      if (customer) {
        if (this.currentCustomer.id != customer.id) {
          this.orderDetailsList = [];
          this.dataSource.data = this.orderDetailsList;
        }
        this.form.get('orderedBy').setValue(undefined);
        this.form.get('orderedByEmail').setValue(undefined);
        this.form.get('orderedByPhoneNumber').setValue(undefined);
        this.currentCustomer = customer;
        this.productFilterDTO.state = customer.state === 'CA' ? 1 : 2;
        this.updateCustomerDetails();
        this.handleSummaryOnChangeEvent();
        this.cd.detectChanges();
      }
    });
  }

  openLocationList(addressType: string) {
    this.dialog.open(LocationListComponent, {
      height: '100%',
      width: '100%',
      data: this.currentCustomer.id
    }).afterClosed().subscribe((location: LocationDTO) => {
      if (location) {
        this.updateAddressDetails(location, addressType);
      }
    });
  }

  openProductListByYearMakeModel() {
    this.dialog.open(ProductListComponent, {
      height: '100%',
      width: '100%',
      data: this.productFilterDTO
    }).afterClosed().subscribe((products: ProductDTO[]) => {
      if (products.length > 0) {
        this.insertProductLineItem(products);
      }
    });
  }

  openProductListByPartNumber() {
    this.dialog.open(ProductListComponent, {
      height: '100%',
      width: '100%',
      data: { state: this.currentCustomer.state === 'CA' ? 1 : 2, partNumber: this.form.value.partNumberFilter }
    }).afterClosed().subscribe((products: ProductDTO[]) => {
      if (products.length > 0) {
        this.insertProductLineItem(products);
      }
    });
  }

  async insertProductLineItem(products: ProductDTO[]) {
    for await (const product of products) {
      if (this.orderDetailsList && this.orderDetailsList.length > 0) {
        let orderDetail = this.orderDetailsList.find(e => e.partNumber === product.partNumber);
        if (orderDetail) {
          let index = this.orderDetailsList.findIndex(e => e.partNumber === product.partNumber);
          orderDetail.orderQuantity = Number(orderDetail.orderQuantity) + 1;
          orderDetail.totalAmount = this.getDiscountedPrice(product) * Number(orderDetail.orderQuantity);
          this.orderDetailsList[index] = orderDetail;
          this.dataSource.data = this.orderDetailsList;
          this.handleSummaryOnChangeEvent();
          this.cd.detectChanges();
        }
        else {
          this.createOrderDetail(product);
        }
      }
      else {
        this.createOrderDetail(product);
      }
    }
  }

  // NJPR - Filter Vendor based on State
  filterVendorByState(vendorCatalogs: VendorCatalog[]): VendorCatalog[] {
    let result: VendorCatalog[] = [];
    vendorCatalogs.forEach(vc => {
      let venCat = {} as VendorCatalog;
      venCat.createdBy = vc.createdBy;
      venCat.createdDate = vc.createdDate;
      venCat.id = vc.id;
      venCat.isActive = vc.isActive;
      venCat.isDeleted = vc.isDeleted;
      venCat.modifiedBy = vc.modifiedBy;
      venCat.modifiedDate = vc.modifiedDate;
      venCat.onHand = vc.onHand;
      venCat.partsLinkNumber = vc.partsLinkNumber;
      venCat.price = vc.price;
      venCat.vendorCode = vc.vendorCode;
      venCat.vendorPartNumber = vc.vendorPartNumber;
      result.push(venCat);
    });

    
    // Get Order State
    let st = this.currentCustomer.state;

    // Filter VendorCatalogs
    vendorCatalogs.forEach(e => {
      let vendor = this.vendorList.find(v => v.vendorCode === e.vendorCode);
      if (vendor) {
        if (vendor.isCAVendor && vendor.isNVVendor) {

        }
        else if (st === 'CA' && !vendor.isCAVendor) {
          result.splice(result.findIndex(tv => tv.vendorCode === e.vendorCode), 1);
        } 
        else if (st === 'NV' && !vendor.isNVVendor) {
          result.splice(result.findIndex(tv => tv.vendorCode === e.vendorCode), 1);
        } 
      }
      else {
        result.splice(result.findIndex(tv => tv.vendorCode === e.vendorCode), 1);
      }
    });

    return result;
  }

  createOrderDetail(product: ProductListDTO) {
    //this.productService.getProductByIdNoStocks(product.id).subscribe(result => {
    this.productService.getProductByIdAndState(product.id, this.currentCustomer.state === 'CA' ? 1 : 2).subscribe(result => {
      if (result) {

        // // NJPR - Filter Vendor based on State
        // // Get Order State
        // let st = this.currentCustomer.state;

        // // Filter VendorCatalogs
        // let tempVC = result.vendorCatalogs;
        
        // tempVC.forEach(e => {
        //   let vendor = this.vendorList.find(v => v.vendorCode === e.vendorCode);
        //   if (vendor) {
        //     if (vendor.isCAVendor && vendor.isNVVendor) {

        //     }
        //     else if (st === 'CA' && !vendor.isCAVendor) {
        //       tempVC.splice(tempVC.findIndex(tv => tv.vendorCode === e.vendorCode), 1);
        //     } 
        //     else if (st === 'NV' && !vendor.isNVVendor) {
        //       tempVC.splice(tempVC.findIndex(tv => tv.vendorCode === e.vendorCode), 1);
        //     } 
        //   }
        // });

        const orderDetail = {} as OrderDetail;
        orderDetail.id = 0; // this.orderDetailsList.length > 0 ? Math.max.apply(Math, this.orderDetailsList.map(function (e) { return e.id; })) + 1 : 1;
        orderDetail.orderId = this.isCreateMode() ? 0 : this.defaults.id;
        orderDetail.productId = product.id;
        orderDetail.orderQuantity = 1; // Default
        orderDetail.imageUrl = product.imageUrl;

        let stocks = result.stocks.filter(e => Number(e.quantity) > 0);
        if (stocks && stocks.length > 0) {
          let currentState = this.currentCustomer.state === 'CA' ? 1 : 2;
          let currentStock = stocks.find(e => e.warehouseId === currentState);
          orderDetail.location = (currentStock) ? currentStock.location : stocks[0].location;
          orderDetail.warehouseLocationId = (currentStock) ? currentStock.warehouseLocationId : stocks[0].warehouseLocationId;
        }
        else orderDetail.location = '';

        orderDetail.partNumber = product.partNumber;
        orderDetail.partDescription = product.partDescription;
        orderDetail.brand = result.brand;
        orderDetail.mainPartsLinkNumber = result.partsLinkNumber;
        orderDetail.mainOEMNumber = result.oemNumber;
        orderDetail.onHandQuantity = result.stock;
        orderDetail.price = result.currentCost;
        orderDetail.yearFrom = result.yearFrom;
        orderDetail.yearTo = result.yearTo;
        orderDetail.listPrice = result.priceLevel1;
        orderDetail.wholesalePrice = this.getWholesalePrice(result);
        orderDetail.discountedPrice = this.getDiscountedPrice(result);
        orderDetail.totalAmount = orderDetail.discountedPrice;
        orderDetail.partSize = result.partSizeId;
        orderDetail.categoryId = result.categoryId;
        orderDetail.partsLinks = result.partsLinks;
        orderDetail.oeMs = result.oeMs;
        orderDetail.unitCost = result.currentCost;
        
        orderDetail.vendorCatalogs = this.filterVendorByState(result.vendorCatalogs); //result.vendorCatalogs;
        orderDetail.vendorCodes = orderDetail.vendorCatalogs.map(e => e.vendorCode).toString(); //result.vendorCodes;

        let tempStocks: WarehouseStock[] = result.stocks;
        
        // if (this.currentCustomer.state === 'CA') {
        //   tempStocks = result.stocks.filter(e => e.warehouseId === 1);
        // }

        orderDetail.onHandQuantity = tempStocks && tempStocks.length > 0 ? tempStocks.map(a => a.quantity).reduce(function(a, b){return a + b;}) : 0;

        orderDetail.stocks = tempStocks ? tempStocks : [];

        this.setDefaultVendor(orderDetail);
        this.orderDetailsList.push(orderDetail);
        this.dataSource.data = this.orderDetailsList;

        this.handleSummaryOnChangeEvent();
        this.cd.detectChanges();
      }
      else {
        this.alertService.failNotification('Product Details', 'Fetching');
      }
    });
  }

  setDefaultVendor(orderDetail: OrderDetail) {
    let defaultVendor = undefined;

    if (orderDetail.onHandQuantity === 0) {
      let vendorCatalogs = orderDetail.vendorCatalogs.filter(e => Number(e.onHand) > 0);
      defaultVendor = vendorCatalogs.find(e => e.price === Math.min.apply(null, vendorCatalogs.map(function (a) { return a.price; })));
      orderDetail.unitCost = defaultVendor ? defaultVendor.price : 0;
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

    // return originalPrice - (originalPrice / 100 * this.form.value.discount)
    return originalPrice - (originalPrice / 100 * this.currentCustomer.discount)
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

  updateAddressDetails(location: LocationDTO, addressType: string) {
    let address = []
    address.push(location.addressLine1);
    address.push(location.addressLine2);

    if (addressType == 'Billing') {
      this.form.get('billAddress').setValue(address.join(' '));
      this.form.get('billCity').setValue(location.city);
      this.form.get('billState').setValue(location.state);
      this.form.get('billZipCode').setValue(location.zipCode);
      this.form.get('billPhoneNumber').setValue(location.phoneNumber);
      this.form.get('billContactName').setValue(location.contactName);
      this.form.get('billZone').setValue(location.zone);
    }
    else if (addressType == 'Shipping') {
      this.form.get('shipAddressName').setValue(location.locationName);
      this.form.get('shipAddress').setValue(address.join(' '));
      this.form.get('shipCity').setValue(location.city);
      this.form.get('shipState').setValue(location.state);
      this.form.get('shipZipCode').setValue(location.zipCode);
      this.form.get('shipPhoneNumber').setValue(location.phoneNumber);
      this.form.get('shipContactName').setValue(location.contactName);
      this.form.get('shipZone').setValue(location.zone);
    }
  }

  updateCustomerDetails(): void {
    let locationList = {} as LocationDTO[];
    let baseLocation: any;
    let billingLocation: any;
    let shippingLocation: any;

    this.form.get('customerName').setValue(this.currentCustomer.customerName);
    this.form.get('accountNumber').setValue(this.currentCustomer.accountNumber);
    this.form.get('phoneNumber').setValue(this.currentCustomer.phoneNumber);
    this.form.get('contactName').setValue(this.currentCustomer.contactName);
    this.form.get('discount').setValue(this.currentCustomer.discount);
    this.form.get('paymentTermId').setValue(this.currentCustomer.paymentTermId);
    this.form.get('priceLevelId').setValue(this.currentCustomer.priceLevelId);

    this.form.get('summaryTaxRate').setValue(this.currentCustomer.taxRate);
    this.form.get('summaryDiscount').setValue(this.currentCustomer.discount);

    this.locationservice.getLocationsList(this.currentCustomer.id).subscribe((result: LocationDTO[]) => {
      if (result) {
        locationList = result

        if (locationList.length > 0) {
          if (locationList.find(e => e.locationTypeId === 1)) {
            baseLocation = locationList.find(e => e.locationTypeId === 1);
          }
          if (locationList.find(e => e.locationTypeId === 3)) {
            billingLocation = locationList.find(e => e.locationTypeId === 3);
          }
          if (locationList.find(e => e.locationTypeId === 4)) {
            shippingLocation = locationList.find(e => e.locationTypeId === 4);
          }
        }

        if (billingLocation) {
          baseLocation = billingLocation;
        }

        let address = []
        address.push(baseLocation.addressLine1);
        address.push(baseLocation.addressLine2);

        this.form.get('billAddress').setValue(address.join(' '));
        this.form.get('billCity').setValue(baseLocation.city);
        this.form.get('billState').setValue(baseLocation.state);
        this.form.get('billZipCode').setValue(baseLocation.zipCode);
        this.form.get('billPhoneNumber').setValue(baseLocation.phoneNumber);
        this.form.get('billContactName').setValue(baseLocation.contactName);
        this.form.get('billZone').setValue(baseLocation.zone);

        if (shippingLocation) {
          baseLocation = shippingLocation;
        }

        address = []
        address.push(baseLocation.addressLine1);
        address.push(baseLocation.addressLine2);

        this.form.get('shipAddressName').setValue(baseLocation.locationName);
        this.form.get('shipAddress').setValue(address.join(' '));
        this.form.get('shipCity').setValue(baseLocation.city);
        this.form.get('shipState').setValue(baseLocation.state);
        this.form.get('shipZipCode').setValue(baseLocation.zipCode);
        this.form.get('shipPhoneNumber').setValue(baseLocation.phoneNumber);
        this.form.get('shipContactName').setValue(baseLocation.contactName);
        this.form.get('shipZone').setValue(baseLocation.zone);
      }
    });
  }

  deleteOrderDetail(row: OrderDetail) {
    this.alertService.deleteNotification('Order Detail').then(answer => {
      if (!answer.isConfirmed) return;
      
      if (row.id === 0) {
        this.orderDetailsList.splice(this.orderDetailsList.findIndex(e => e.partNumber === row.partNumber), 1);
        this.dataSource.data = this.orderDetailsList;
        this.handleSummaryOnChangeEvent();
        this.cd.detectChanges();
      }
      else {
        // --->>> This will not be called since there is no delete in Update Order
      }
    });
  }

  deleteQuoteDetail(row: OrderDetail) {
    this.alertService.deleteNotification('Order Detail').then(answer => {
      if (!answer.isConfirmed) return;
      
      if (row.id === 0) {
        this.orderDetailsList.splice(this.orderDetailsList.findIndex(e => e.partNumber === row.partNumber), 1);
        this.dataSource.data = this.orderDetailsList;
        this.handleSummaryOnChangeEvent();
        this.cd.detectChanges();
      }
      else {
        this.orderService.deleteOrderDetail(row.id).subscribe(result => {
          if (result) {
            this.orderDetailsList.splice(this.orderDetailsList.findIndex(e => e.id === row.id), 1);
            this.dataSource.data = this.orderDetailsList;
            this.handleSummaryOnChangeEvent();
            this.cd.detectChanges();
            this.updateOrderSummary();
          }
          else this.alertService.failNotification("Quote Detail", "Delete");
        });
      }
    });
  }

  flagOrderDetail(row: OrderDetail) {
    this.alertService.showAllButtons('Order Detail').then(answer => {
      if (!answer.isConfirmed && answer.isDismissed) return;

      this.orderDetailsList.find(e => e.id === row.id).orderQuantity = 0;
      this.orderDetailsList.find(e => e.id === row.id).wholesalePrice = 0;
      this.orderDetailsList.find(e => e.id === row.id).statusId = answer.isConfirmed ? 1 : 2;
      this.dataSource.data = this.orderDetailsList;
      this.handleSummaryOnChangeEvent();
      this.cd.detectChanges();
    });;
  }

  setBackOrder(deliveryDate: any) {
    this.prepareBackOrderForUpdate(deliveryDate);
    
    this.selection.selected.forEach(row => {
      this.orderDetailsList.find(e => e.partNumber === row.partNumber).orderQuantity = 0;
      this.orderDetailsList.find(e => e.partNumber === row.partNumber).wholesalePrice = 0;
      this.orderDetailsList.find(e => e.partNumber === row.partNumber).statusId = 3;
      this.orderDetailsList.find(e => e.partNumber === row.partNumber).warehouseTracking = 'Back Ordered';
    });

    this.dataSource.data = this.orderDetailsList;
    this.handleSummaryOnChangeEvent();
    this.cd.detectChanges();
  }

  prepareBackOrderForUpdate(deliveryDate: any) {
    const order = {} as Order;
    const orderDetails = JSON.parse(JSON.stringify(this.selection.selected)) as typeof this.selection.selected;
    this.mapFormValuesToOrder(order, orderDetails);
    order.isQuote = false;
    const backOrder = JSON.parse(JSON.stringify(order)) as typeof order;
    
    //Set IDs to 0 for Create
    backOrder.id = 0;
    backOrder.orderDetails.forEach(e => {
      e.id = 0;
    });

    //Set new Delivery Date
    backOrder.deliveryDate = deliveryDate;

    backOrder.isPrinted = false;

    this.handleBackOrderSummary(backOrder);
    this.backOrder = backOrder;
  }

  mapSelectedToOrderDetails(): OrderDetail[] {
    let orderDetails: OrderDetail[] = [];
    this.selection.selected.forEach(e => {
      const od = {} as OrderDetail;
      od.brand = e.brand;
      od.buyOutOrder = e.buyOutOrder;
      od.carrier = e.carrier;
      od.categoryId = e.categoryId;
      od.createdBy = e.createdBy;
      od.createdDate = e.createdDate;
      od.discountedPrice = e.discountedPrice;
      od.id = 0;
      od.imageUrl = e.imageUrl;
      od.isActive = e.isActive;
      od.isDeleted = e.isDeleted;
      od.itemCatalog = e.itemCatalog;
      od.listPrice = e.listPrice;
      od.location = e.location;
      od.mainOEMNumber = e.mainOEMNumber;
      od.mainPartsLinkNumber = e.mainPartsLinkNumber;
      od.modifiedBy = e.modifiedBy;
      od.modifiedDate = e.modifiedDate;
      od.oeMs = e.oeMs;
      od.onHandQuantity = e.onHandQuantity;
      od.order = e.order;
      od.orderId = e.orderId;
      od.orderQuantity = e.orderQuantity;
      od.originalInvoiceNumber = e.originalInvoiceNumber;
      od.partDescription = e.partDescription;
      od.partNumber = e.partNumber;
      od.partSize = e.partSize;
      od.partsLinks = e.partsLinks;
      od.pickedQuantity = e.pickedQuantity;
      od.price = e.price;
      od.product = e.product;
      od.productId = e.productId;
      od.restockingAmount = e.restockingAmount;
      od.restockingFee = e.restockingFee;
      od.salesOrderNumber = e.salesOrderNumber;
      od.salesRepresentative = e.salesRepresentative;
      od.scannedQuantity = e.scannedQuantity;
      od.shipCharge = e.shipCharge;
      od.shipDate = e.shipDate;
      od.shippedQuantity = e.shippedQuantity;
      od.statusId = e.statusId;
      od.stocks = e.stocks;
      od.toShip = e.toShip;
      od.totalAmount = e.totalAmount;
      od.unitCost = e.unitCost;
      od.vehicle = e.vehicle;
      od.vendorCatalogs = e.vendorCatalogs;
      od.vendorCode = e.vendorCode;
      od.vendorCodes = e.vendorCodes;
      od.vendorInfo = e.vendorInfo;
      od.vendorOnHand = e.vendorOnHand;
      od.vendorPartNumber = e.vendorPartNumber;
      od.vendorPrice = e.vendorPrice;
      od.warehouseLocation = e.warehouseLocation;
      od.warehouseTracking = e.warehouseTracking;
      od.wholesalePrice = e.wholesalePrice;
      od.yearFrom = e.yearFrom;
      od.yearTo = e.yearTo;

      orderDetails.push(od);
    });

    return orderDetails;
  }

  minusQuantity(row: OrderDetail) {
    let orderDetail = this.orderDetailsList.find(e => e.id === row.id);
    if (orderDetail.orderQuantity === 1) return;
    this.orderDetailsList.find(e => e.id === row.id).orderQuantity = orderDetail.orderQuantity - 1;
    this.orderDetailsList.find(e => e.id === row.id).totalAmount = (orderDetail.orderQuantity) * orderDetail.discountedPrice;
    this.handleSummaryOnChangeEvent();
    this.cd.detectChanges();
  }

  addQuantity(row: OrderDetail) {
    let orderDetail = this.orderDetailsList.find(e => e.id === row.id);
    this.orderDetailsList.find(e => e.id === row.id).orderQuantity = orderDetail.orderQuantity + 1;
    this.orderDetailsList.find(e => e.id === row.id).totalAmount = (orderDetail.orderQuantity) * orderDetail.discountedPrice;
    this.handleSummaryOnChangeEvent();
    this.cd.detectChanges();
  }

  onLabelChange(change: MatSelectChange, row: OrderDetail) {
    row.vendorCode = change.value.vendorCode;
    row.vendorPartNumber = change.value.vendorPartNumber;
    row.vendorPrice = change.value.price;
    row.vendorOnHand = change.value.onHand;
    this.cd.detectChanges();
  }

  handleBackOrderSummary(order: Order) {
    order.orderDetails.forEach(e => {
      let totalPrice = e.wholesalePrice * e.orderQuantity;
      e.discountedPrice = e.wholesalePrice - (e.wholesalePrice / 100 * this.form.value.summaryDiscount);
      e.totalAmount = totalPrice - (totalPrice / 100 * this.form.value.summaryDiscount);
    });

    let discountAmount = (order.orderDetails.length > 0) ? order.orderDetails.map(e => e.discountedPrice * e.orderQuantity).reduce(function (prev, next) { return prev + next }) : 0.00;
    let subTotalAmount = (order.orderDetails.length > 0) ? order.orderDetails.map(e => e.wholesalePrice * e.orderQuantity).reduce(function (prev, next) { return prev + next }) : 0;
    let totalAmount = (order.orderDetails.length > 0) ? order.orderDetails.map(e => e.totalAmount).reduce(function (prev, next) { return prev + next }) : 0;
    let totalTax = (this.form.value.summaryTaxRate > 0) ? totalAmount * this.form.value.summaryTaxRate / 100 : 0.00;

    order.discount = this.form.value.summaryDiscount;
    order.subTotalAmount = subTotalAmount;
    order.totalAmount = (order.orderDetails.length > 0) ? totalAmount + totalTax : 0 + totalTax;
    order.taxRate = this.form.value.summaryTaxRate;
    order.totalTax = totalTax;
    order.balance = order.totalAmount;


    // this.summaryDiscount = this.form.value.summaryDiscount;
    // this.summarySubTotal = subTotalAmount; // - totalTax;
    // this.summaryTax = totalTax;
    // this.summaryTaxRate = this.form.value.summaryTaxRate;
    // this.summaryTotal = (order.orderDetails.length > 0) ? totalAmount + totalTax : 0 + totalTax;


    // this.form.get('summaryDiscountAmount').setValue((subTotalAmount - discountAmount).toFixed(2));
    // this.form.get('summarySubTotal').setValue(this.summarySubTotal.toFixed(2));
    // this.form.get('summaryTax').setValue(this.summaryTax.toFixed(2));
    // this.form.get('summaryTotal').setValue(this.summaryTotal.toFixed(2));
  }

  handleSummaryOnChangeEvent() {
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
    this.handleSummaryOnChangeEvent();
    this.cd.detectChanges();
  }

  handleListPriceOnChangeEvent(row: OrderDetail, event: any) {
    let orderDetail = this.orderDetailsList.find(e => e.partNumber === row.partNumber);
    orderDetail.listPrice = Number(event.target.value);
    this.cd.detectChanges();
  }

  handleWholesalePriceOnChangeEvent(row: OrderDetail, event: any) {
    let orderDetail = this.orderDetailsList.find(e => e.partNumber === row.partNumber);
    orderDetail.wholesalePrice = Number(event.target.value);
    this.handleSummaryOnChangeEvent();
    this.cd.detectChanges();
  }

  handleWarehouseTrackingOnChangeEvent(row: OrderDetail, event: any) {
    let orderDetail = this.orderDetailsList.find(e => e.partNumber === row.partNumber);
    orderDetail.warehouseTracking = event.target.value;
    //this.handleSummaryOnChangeEvent();
    this.cd.detectChanges();
  }

  radioButtonChange(contact: Contact) {
    this.form.get('orderedBy').setValue(contact.contactName);
    this.form.get('orderedByEmail').setValue(contact.email);
    this.form.get('orderedByPhoneNumber').setValue(contact.phoneNumber);
  }

  getTooltip(row) {
    let result = '';
    if (row.stocks) {
      row.stocks.forEach(element => {
        result = '<tr>' + element.warehouseLocationId + ' - ' + element.quantity + '</tr>'
      });
    }
    return result;
  }

  getOffList(row: OrderDetail) {
    let result = (row.listPrice - row.wholesalePrice) / row.listPrice * 100;
    return result.toFixed(2) + '%';
  }

  getProfit(row: OrderDetail) {
    //var cost = row.unitCost ? row.unitCost : row.curr
    let result = (row.wholesalePrice - (row.unitCost)) / row.wholesalePrice * 100;
    return result.toFixed(2) + '%';
  }

  getZoneValue(event: any) {
    this.form.get('shipZone').setValue('');
    this.zoneService.getZoneByZipCode(event.target.value).subscribe(result => {
      if (result) {
        this.form.get('shipZone').setValue(result.binCode.trim());
      }
      else {
        this.alertService.validationFailedNotification('ZipCode Error', 'Thera are no matching Zone for ZipCode ' + event.target.value);
      }
    }, () => {
      this.alertService.validationFailedNotification('ZipCode Error', 'Thera are no matching Zone for ZipCode ' + event.target.value);
    });
  }

  // setManualVendor(row: OrderDetail, valC: string, valN: string, valP: string, valQ: string) {
  //   if (valC.length === 0) {
  //     this.alertService.validationRequiredNotification('Vendor Code is required.')
  //     return;
  //   }

  //   if (valN.length === 0) {
  //     this.alertService.validationRequiredNotification('Vendor Part Number is required.')
  //     return;
  //   }

  //   if (Number(valP) === 0) {
  //     this.alertService.validationRequiredNotification('Vendor Price should be grater than zero.')
  //     return;
  //   }

  //   row.vendorCode = valC;
  //   row.vendorPartNumber = valN;
  //   row.vendorPrice = Number(valP);
  //   row.vendorOnHand = Number(valQ);

  //   this.inputC = '';
  //   this.inputN = '';
  //   this.inputP = 0;
  //   this.inputQ = 1;
  //   this.cd.detectChanges();
  // }

  // openVendorDialog(row: any) {
  //   if (row.onHandQuantity > 0 && row.statusId !== 7) return;

  //   // // NJPR - Filter Vendor based on State
  //   // // Get Order State
  //   // let st = this.currentCustomer.state;

  //   // // Filter VendorCatalogs
  //   // let tempVC = row.vendorCatalogs;
    
  //   // tempVC.forEach(e => {
  //   //   let vendor = this.vendorList.find(v => v.vendorCode === e.vendorCode);
  //   //   if (vendor) {
  //   //     if (vendor.isCAVendor && vendor.isNVVendor) {

  //   //     }
  //   //     else if (st === 'CA' && !vendor.isCAVendor) {
  //   //       tempVC.splice(tempVC.findIndex(tv => tv.vendorCode === e.vendorCode), 1);
  //   //     } 
  //   //     else if (st === 'NV' && !vendor.isNVVendor) {
  //   //       tempVC.splice(tempVC.findIndex(tv => tv.vendorCode === e.vendorCode), 1);
  //   //     } 
  //   //   }
  //   // });

  //   row.vendorCatalogs = this.filterVendorByState(row.vendorCatalogs);

  //   this.dialog.open(VendorInputDialog, {
  //     // height: '30%',
  //     // width: '30%',
  //     data: row
  //   }).afterClosed().subscribe((vendor: any) => {
  //     if (vendor) {
  //       row.vendorCode = vendor.vendorCode;
  //       row.vendorPartNumber = vendor.vendorPartNumber;
  //       row.vendorPrice = Number(vendor.vendorPrice);
  //       row.vendorOnHand = Number(vendor.vendorQuantity);
  //       row.unitCost = row.vendorPrice;
  //     }
  //   });
  // }

  addNewContact() {
    this.dialog.open(ContactCreateUpdateComponent, {
      data: this.currentCustomer.id
    }).afterClosed().subscribe((contact = {} as Contact) => {
      if (contact) {
        this.contactService.createContact(contact).subscribe((result: Contact[]) => {
          if (result) {
            this.customerService.getCustomerById(this.currentCustomer.id).subscribe((result: CustomerDTO) => {
              if (result) {
                this.currentCustomer = result;
                const newContact = this.currentCustomer.contacts.reduce((prev, current) => (prev.id > current.id) ? prev : current)
                if (newContact) {
                  this.selectedContact = newContact.contactName;
                  this.radioButtonChange(newContact);
                }
                this.cd.detectChanges();
                this.alertService.successNotification("Contact", "Create");
              }
              else this.alertService.failNotification("Contact", "Create");
            })
          }
          else this.alertService.failNotification("Contact", "Create");
        });
      }
    });
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  cancel() {
    if (this.defaults.orderStatusId === 1) {
      this.alertService.cancelNotification("Order").then(answer => {
        if (!answer.isConfirmed) { return; }
        this.dialogRef.close(undefined);
      });
    }
    else this.dialogRef.close(undefined);

    // if (this.isCreateMode()) {
    //   if (this.dataSource.data.length > 0) {
    //     this.save('Quote');
    //   }
    //   else {
    //     this.alertService.cancelNotification("Order").then(answer => {
    //       if (!answer.isConfirmed) { return; }
    //       this.dialogRef.close(undefined);
    //     });
    //   }
    // }
    // else {
    //   if (this.defaults.orderStatusId === 1) {
    //     this.alertService.cancelNotification("Order").then(answer => {
    //       if (!answer.isConfirmed) { return; }
    //       this.dialogRef.close(undefined);
    //     });
    //   }
    //   else this.dialogRef.close(undefined);
    // }
  }

  showfilters() {
    // return true;
    let result = this.currentCustomer.id !== undefined && this.filteredYears$ && this.filteredMakes$ && this.filteredModels$ && this.filteredCategories$;
    this.cd.detectChanges();
    return !result;

  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  displayPaymentHistory(row: PaymentHistoryDTO) {
    let result = '';
    result += 'Payment Date: ' + this.formatDate(row.paymentDate) + ' - ';
    result += row.invoiceBalance > 0 ? 'Partially Paid: ' : 'Fully Paid: ';
    result += row.paymentType + ' $' + this.formatCurrency(row.invoicePaymentAmount);
    result += (row.paymentType !== 'Credit Memo' && row.customerCreditAmountUsed > 0) ? ' Credit: $' + this.formatCurrency(row.customerCreditAmountUsed) : '';
    result += row.invoiceBalance > 0 ? ' Balance: $' + this.formatCurrency(row.invoiceBalance) : '';
    result += (row.paymentType === 'Credit Memo' && (row.linkedInvoiceNumber && row.linkedInvoiceNumber.length > 0)) ? ' --> Credit Memo Applied Invoice Number(s): ' + row.linkedInvoiceNumber : '';
    return result;
  }

  displayCreditMemoHistory() {
    let result = 'Original Invoice: ' + this.defaults.originalInvoiceNumber + ' - ';
    result += 'Amount Used: ' + this.formatCurrency(this.defaults.amountPaid) + ' - ';
    result += 'Balance: ' + this.formatCurrency(this.defaults.balance);
    result += (this.defaults.orderStatusId === 5 && (this.defaults.linkedInvoiceNumber && this.defaults.linkedInvoiceNumber.length > 0)) ? ' --> Order Applied Invoice Number(s): ' + this.defaults.linkedInvoiceNumber : '';
    return result;
  }

  displayOverpaymentOrigin() {
    let result = this.defaults.invoiceNotes ? this.defaults.invoiceNotes : '';
    return result;
  }

  displayOverpaymentHistory() {
    let result = 'Amount Used: ' + this.formatCurrency(this.defaults.amountPaid) + ' - ';
    result += 'Balance: ' + this.formatCurrency(this.defaults.balance);
    result += ((this.defaults.orderStatusId === 5 || this.defaults.orderStatusId === 7) && (this.defaults.linkedInvoiceNumber && this.defaults.linkedInvoiceNumber.length > 0)) ? ' --> Order Applied Invoice Number(s): ' + this.defaults.linkedInvoiceNumber : '';
    return result;
  }
  
  // createBackOrder() {
  //   if (this.selection.selected.length === 0) return;
  //   this.dialog.open(BackOrderComponent, {
  //     // height: '80%',
  //     // width: '70%',
  //     //data: creditMemoData
  //   }).afterClosed().subscribe((deliveryDate: any) => {
  //     if (deliveryDate) {
  //       this.setBackOrder(deliveryDate);
  //     }
  //   });
  // }

  // creditMemo() {
  //   if (this.selection.selected.length === 0) return;

  //   const creditMemoData = {} as Order;
  //   this.mapCreditMemoDataFromOriginalOrder(creditMemoData);

  //   creditMemoData.subTotalAmount = 0;
  //   creditMemoData.totalAmount = 0;
  //   creditMemoData.totalTax = 0;
  //   // creditMemoData.amountPaid *= -1;

  //   let cmOrderDetails: OrderDetail[] = [];
    
  //   this.selection.selected.forEach(e => {
  //     const cmOrderDetail = {} as OrderDetail;
  //     this.mapCreditMemoOrderDetailFromSelected(cmOrderDetail, e);

  //     creditMemoData.subTotalAmount += cmOrderDetail.totalAmount;
  //     creditMemoData.totalTax += (creditMemoData.taxRate > 0) ? cmOrderDetail.totalAmount * creditMemoData.taxRate / 100 : 0;
  //     creditMemoData.totalAmount += cmOrderDetail.totalAmount + ((creditMemoData.taxRate > 0) ? cmOrderDetail.totalAmount * creditMemoData.taxRate / 100 : 0);
  //     cmOrderDetails.push(cmOrderDetail);
  //   });

  //   creditMemoData.orderDetails = cmOrderDetails;

  //   this.dialog.open(CreditMemoComponent, {
  //     height: '80%',
  //     width: '70%',
  //     data: creditMemoData
  //   }).afterClosed().subscribe((creditMemo: Order) => {
  //     if (creditMemo) {
  //       this.alertService.showBlockUI("Saving Credit Memo...");
  //       this.orderService.createCreditMemo(creditMemo).subscribe((result: boolean) => {
  //         if (result) {
  //           this.alertService.hideBlockUI();
  //           this.alertService.successNotification("Credit Memo", "Create");
  //         }
  //         else {
  //           this.alertService.hideBlockUI();
  //           this.alertService.failNotification("Credit Memo", "Create");
  //         }
  //       });
  //     }
  //     else {
  //       this.alertService.hideBlockUI();
  //       //this.dialogRef.close(undefined);
  //     }
  //   });
  // }

  mapCreditMemoOrderDetailFromSelected(to: OrderDetail, fr: OrderDetail) {
    to.id = fr.id;
    to.isActive = fr.isActive;
    to.isDeleted = fr.isDeleted;
    to.createdBy = fr.createdBy;
    to.createdDate = fr.createdDate;
    to.modifiedBy = fr.modifiedBy;
    to.modifiedDate = fr.modifiedDate;
    to.orderId = fr.orderId;
    to.order = fr.order;
    to.productId = fr.productId;
    to.product = fr.product;
    to.categoryId = fr.categoryId;
    to.orderQuantity = fr.orderQuantity;
    to.onHandQuantity = fr.onHandQuantity;
    to.location = fr.location;
    to.partNumber = fr.partNumber;
    to.partDescription = fr.partDescription;
    to.brand = fr.brand;
    to.mainPartsLinkNumber = fr.mainPartsLinkNumber;
    to.mainOEMNumber = fr.mainOEMNumber;
    to.vendorCode = fr.vendorCode;
    to.vendorPartNumber = fr.vendorPartNumber;
    to.vendorPrice = fr.vendorPrice;
    to.vendorOnHand = fr.vendorOnHand;
    to.yearFrom = fr.yearFrom;
    to.yearTo = fr.yearTo;
    to.partsLinks = fr.partsLinks;
    to.oeMs = fr.oeMs;
    to.vendorCodes = fr.vendorCodes;
    to.imageUrl = fr.imageUrl;
    to.stocks = fr.stocks;
    to.vendorCatalogs = fr.vendorCatalogs;
    to.vehicle = fr.vehicle;
    to.salesRepresentative = fr.salesRepresentative;
    to.warehouseLocation = fr.warehouseLocation;
    to.vendorInfo = fr.vendorInfo;
    to.warehouseTracking = fr.warehouseTracking;
    to.carrier = fr.carrier;
    to.salesOrderNumber = fr.salesOrderNumber;
    to.originalInvoiceNumber = fr.originalInvoiceNumber;
    to.buyOutOrder = fr.buyOutOrder;
    to.partSize = fr.partSize;
    to.statusId = fr.statusId;
    to.shippedQuantity = fr.shippedQuantity;
    to.scannedQuantity = fr.scannedQuantity;
    to.pickedQuantity = fr.pickedQuantity;
    to.itemCatalog = fr.itemCatalog;
    to.toShip = fr.toShip;
    to.price = fr.price;
    to.unitCost = fr.unitCost;
    to.shipCharge = fr.shipCharge;
    to.shipDate = fr.shipDate;

    to.listPrice = fr.listPrice;

    // Reverse Values
    to.wholesalePrice = (fr.wholesalePrice > 0) ? fr.wholesalePrice * -1 : 0;
    to.discountedPrice = (fr.discountedPrice > 0) ? fr.discountedPrice * -1 : 0;
    to.totalAmount = (fr.totalAmount > 0) ? fr.totalAmount * -1 : 0;
  }

  mapCreditMemoDataFromOriginalOrder(cm: Order) {
    cm.id = this.originalOrder.id;
    cm.isActive = this.originalOrder.isActive;
    cm.isDeleted = this.originalOrder.isDeleted;
    cm.createdBy = this.originalOrder.createdBy;
    cm.createdDate = this.originalOrder.createdDate;
    cm.modifiedBy = this.originalOrder.modifiedBy;
    cm.modifiedDate = this.originalOrder.modifiedDate;
    cm.customerId = this.originalOrder.customerId;
    cm.customer = this.originalOrder.customer;
    cm.paymentTermId = this.originalOrder.paymentTermId;
    cm.paymentTerm = this.originalOrder.paymentTerm;
    cm.warehouseId = this.originalOrder.warehouseId;
    cm.warehouse = this.originalOrder.warehouse;
    cm.orderNumber = this.originalOrder.orderNumber;
    cm.quoteNumber = this.originalOrder.quoteNumber;
    cm.customerName = this.originalOrder.customerName;
    cm.accountNumber = this.originalOrder.accountNumber;
    cm.paymentTermName = this.originalOrder.paymentTermName;
    cm.priceLevelId = this.originalOrder.priceLevelId;
    cm.priceLevelName = this.originalOrder.priceLevelName;
    cm.warehouseName = this.originalOrder.warehouseName;
    cm.orderDate = this.originalOrder.orderDate;
    cm.user = this.originalOrder.user;
    cm.orderStatusId = this.originalOrder.orderStatusId;
    cm.orderStatusName = this.originalOrder.orderStatusName;
    cm.orderedBy = this.originalOrder.orderedBy;
    cm.orderedByEmail = this.originalOrder.orderedByEmail;
    cm.orderedByPhoneNumber = this.originalOrder.orderedByPhoneNumber;
    cm.orderedByNotes = this.originalOrder.orderedByNotes;
    cm.deliveryType = this.originalOrder.deliveryType;
    cm.deliveryDate = this.originalOrder.deliveryDate;
    cm.deliveryRoute = this.originalOrder.deliveryRoute;
    cm.isQuote = this.originalOrder.isQuote;
    cm.orderDetails = []; //this.originalOrder.orderDetails;
    cm.email = this.originalOrder.email;
    cm.phoneNumber = this.originalOrder.phoneNumber;
    cm.billAddress = this.originalOrder.billAddress;
    cm.billCity = this.originalOrder.billCity;
    cm.billState = this.originalOrder.billState;
    cm.billZipCode = this.originalOrder.billZipCode;
    cm.billZone = this.originalOrder.billZone;
    cm.billPhoneNumber = this.originalOrder.billPhoneNumber;
    cm.billContactName = this.originalOrder.billContactName;
    cm.shipAddressName = this.originalOrder.shipAddressName;
    cm.shipAddress = this.originalOrder.shipAddress;
    cm.shipCity = this.originalOrder.shipCity;
    cm.shipState = this.originalOrder.shipState;
    cm.shipZipCode = this.originalOrder.shipZipCode;
    cm.shipZone = this.originalOrder.shipZone;
    cm.shipPhoneNumber = this.originalOrder.shipPhoneNumber;
    cm.shipContactName = this.originalOrder.shipContactName;
    cm.isHoldInvoice = this.originalOrder.isHoldInvoice;
    cm.invoiceNumber = this.originalOrder.invoiceNumber;
    cm.fwd = this.originalOrder.fwd;
    cm.originalInvoiceNumber = this.originalOrder.originalInvoiceNumber;
    cm.orderType = this.originalOrder.orderType;
    cm.handlingType = this.originalOrder.handlingType;
    cm.creditType = this.originalOrder.creditType;
    cm.singleInvoice = this.originalOrder.singleInvoice;
    cm.purchaseOrderNumber = this.originalOrder.purchaseOrderNumber;
    cm.type = this.originalOrder.type;
    cm.category = this.originalOrder.category;
    cm.carrier = this.originalOrder.carrier;
    cm.salesRepresentative1 = this.originalOrder.salesRepresentative1;
    cm.salesRepresentative2 = this.originalOrder.salesRepresentative2;
    cm.invoiceNotes = this.originalOrder.invoiceNotes;
    cm.particular = this.originalOrder.particular;
    cm.paymentReference = this.originalOrder.paymentReference;
    cm.postedBy = this.originalOrder.postedBy;
    cm.accountingPaidBy = this.originalOrder.accountingPaidBy;
    cm.transactionDate = this.originalOrder.transactionDate;
    cm.postDate = this.originalOrder.postDate;
    cm.paymentDate = this.originalOrder.paymentDate;
    cm.debit = this.originalOrder.debit;
    cm.credit = this.originalOrder.credit;
    cm.quantity = this.originalOrder.quantity;
    cm.currentCost = this.originalOrder.currentCost;
    cm.amountPaid = this.originalOrder.amountPaid;
    cm.balance = this.originalOrder.balance;
    cm.payment = this.originalOrder.payment;
    cm.listPrice = this.originalOrder.listPrice;

    cm.discount = this.originalOrder.discount; // This is in Percentage
    cm.taxRate = this.originalOrder.taxRate; // This is in Percentage

    // Reverse Values
    cm.totalTax = (this.originalOrder.totalTax > 0) ? this.originalOrder.totalTax * -1 : 0;
    cm.subTotalAmount = (this.originalOrder.subTotalAmount > 0) ? this.originalOrder.subTotalAmount * -1 : 0;
    cm.totalAmount = (this.originalOrder.totalAmount > 0) ? this.originalOrder.totalAmount * -1 : 0;
  }

  getStockAvailability(row: OrderDetail) {
    if (this.isUpdateMode() && row.id > 0) {
      if (!row.statusId) return 'Avaliable';

      switch (row.statusId) {
        case 1 : return 'Customer Cancelled';
        case 2 : return 'Out of Stock';
        // case 3 : return 'Back Ordered';
        // case 4 : return 'Delivered';
        // case 5 : return 'Credit Memo';
        // case 6 : return 'Retry';
        // case 7 : return 'Skipped Item';
        default: return 'Available';
      }
    }

    let stocks = row.onHandQuantity;
    if (stocks === 0 && row.vendorCatalogs && row.vendorCatalogs.length > 0) {
      stocks = row.vendorCatalogs.map(e => e.onHand).reduce(function(a, b){ return a + b; });
    }
    return (stocks > 0) ? 'Available' : 'Out of stock';
  }
}
