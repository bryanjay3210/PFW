import { ChangeDetectorRef, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { scaleFadeIn400ms } from 'src/@vex/animations/scale-fade-in.animation';
import { scaleIn400ms } from 'src/@vex/animations/scale-in.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { Link } from 'src/@vex/interfaces/link.interface';
import { trackById } from 'src/@vex/utils/track-by';
import moment from 'moment';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { UntilDestroy } from '@ngneat/until-destroy';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { LookupService } from 'src/services/lookup.service';
import { PriceLevel } from 'src/services/interfaces/priceLevel.model';
import { PaymentTerm } from 'src/services/interfaces/paymentTerm.model';
import { SalesRepresentative } from 'src/services/interfaces/salesRepresentative.model';
import { AlertService } from 'src/services/alert.service';
import { countries, Country } from 'src/static-data/country-data';
import { State } from 'src/static-data/state-data';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { Customer, CustomerNote, User } from 'src/services/interfaces/models';
import { CustomerNoteService } from 'src/services/customernote.service';
import { MatTableDataSource } from '@angular/material/table';
import { ReportService } from 'src/services/report.service';
import { formatCurrency } from '@angular/common';


export interface CountryState {
  name: string;
  population: string;
  flag: string;
}

@UntilDestroy()
@Component({
  selector: 'vex-customer-create-update',
  templateUrl: './customer-create-update.component.html',
  styleUrls: ['./customer-create-update.component.scss'],
  animations: [
    scaleIn400ms,
    fadeInRight400ms,
    stagger40ms,
    fadeInUp400ms,
    scaleFadeIn400ms
  ]
})

export class CustomerCreateUpdateComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()

  columns: TableColumn<CustomerNote>[] = [
    { label: 'Date', property: 'createdDate', type: 'text', visible: true },
    { label: 'Note', property: 'notes', type: 'text', visible: true },
    { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";

  dataSource: MatTableDataSource<CustomerNote> | null;
  // Lookup Data
  customerTypeList: Lookup[];
  locationTypeList: Lookup[];
  priceLevelList: PriceLevel[];
  paymentTermList: PaymentTerm[];
  salesRepresentativeList: SalesRepresentative[] = [];
  salesRepresentativeOutList: SalesRepresentative[] = [];
  salesRepresentativeInList: SalesRepresentative[] = [];

  pageSizeOptions: number[] = [5, 10, 20, 50];
  pageSize = 10;
  isHoldAccount: boolean = false;

  static id = 100;
  inputType = 'password';
  visible = false;

  links: Link[] = [
    {
      label: 'Customer Details',
      route: '../apps/customers/details'
    },
    {
      label: 'Contacts',
      route: '../apps/customers/contacts'
    },
    {
      label: 'Locations',
      route: '../apps/customers/locations'
    }
  ];

  trackById = trackById;

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  countryList: Country[] = countries;
  stateList: State[] = undefined;
  selectedCountry: Country = undefined;
  selectedState: State = undefined;

  customerNoteCtrl = new UntypedFormControl;
  countryCtrl: UntypedFormControl;
  filteredCountries$: Observable<Country[]>;
  stateCtrl: UntypedFormControl;
  filteredStates$: Observable<State[]>;

  sortColumn: string = '';
  sortOrder: string = '';

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  bypassCreditLimit : boolean = false;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Customer,
    private dialogRef: MatDialogRef<CustomerCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private lookupService: LookupService,
    private customerNoteService: CustomerNoteService,
    private reportService: ReportService, 
    private alertService: AlertService,
    private changeDetector: ChangeDetectorRef) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerManagement);
    this.access = modulePermission.accessTypeId;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();
    this.getLookups();
    
    if (this.defaults) {
      this.mode = 'update';
      this.isHoldAccount = this.defaults.isHoldAccount;
      this.selectedCountry = this.countryList.find(c => c.name === this.defaults.country);
      this.bypassCreditLimit = this.defaults.isBypassCreditLimit;
      this.getCustomerNotes();
      
      if (this.selectedCountry) this.stateList = this.selectedCountry.states;
      this.selectedState = this.stateList.find(s => s.code === this.defaults.state);
      this.stateCtrl.setValue(this.defaults.state);
    } else {
      this.defaults = {} as Customer;
      this.selectedCountry = this.countryList.find(c => c.code === 'US');
      if (this.selectedCountry) this.stateList = this.selectedCountry.states;
      this.selectedState = undefined;
    }

    this.countryCtrl.setValue(this.selectedCountry.name);

    this.form = this.fb.group({
      id: [CustomerCreateUpdateComponent.id++],
      accountNumber: [this.defaults.accountNumber || ''],
      customerName: [this.defaults.customerName || '', Validators.required],
      customerTypeId: [this.defaults.customerTypeId || undefined, Validators.required],
      phoneNumber: [this.defaults.phoneNumber || '', Validators.required],
      faxNumber: [this.defaults.faxNumber || '', Validators.required],
      contactName: [this.defaults.contactName || '', Validators.required],
      addressLine1: [this.defaults.addressLine1 || '', Validators.required],
      addressLine2: [this.defaults.addressLine2 || ''],
      city: [this.defaults.city || '', Validators.required],
      state: [this.defaults.state || ''],
      country: [this.defaults.country || this.selectedCountry.name, Validators.required],
      zipCode: [this.defaults.zipCode || '', Validators.required],
      email: [this.defaults.email || '', Validators.required],
      priceLevelId: [this.defaults.priceLevelId || undefined, Validators.required],
      paymentTermId: [this.defaults.paymentTermId || undefined, Validators.required],
      creditLimit: [this.defaults.creditLimit || 0],
      taxRate: [this.defaults.taxRate || 0],
      overBalance: [this.defaults.overBalance || 0],
      isHoldAccount: [this.defaults.isHoldAccount || false],
      discount: [this.defaults.discount || 0],
      sellersPermit: [this.defaults.sellersPermit || ''],
      crossStreet: [this.defaults.crossStreet || ''],
      salesRepresentativeOutId: [this.defaults.salesRepresentativeOutId || undefined],
      salesRepresentativeInId: [this.defaults.salesRepresentativeInId || undefined],
    });

    if (this.isUpdateMode()) {
      this.getCustomerStatement();
    }
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  getCustomerNotes() {
    this.customerNoteService.getCustomerNotesByCustomerId(this.defaults.id).subscribe({
      next: (result) => {
        if (result) {
          this.dataSource.data = result;
        }
      },
      error: (e) => {
        this.alertService.failNotification("Customer Notes", "Fetch");
        console.error(e)
      },
      complete: () => console.info('complete') 
    });
  }

  getCustomerStatement() {
    this.form.get('overBalance').setValue(0);
    let rawDate = new Date().setHours(0,0,0,0);
    let currentDate = new Date(rawDate).toISOString();
    this.reportService.getStatementReport(currentDate, this.defaults.paymentTermId, [ this.defaults.id ]).subscribe({
      next: (result) => {
        if (result) {
          this.form.get('overBalance').setValue(this.formatCurrency(result[0].totalDue));
        }
      },
      error: (e) => {
        this.alertService.failNotification("Customer Balance", "Fetch");
        console.error(e)
      },
      complete: () => console.info('complete') 
    });
  }

  createNote() {
    if (this.customerNoteCtrl.value.trim().length === 0) return;
    this.alertService.createNotification('Customer Note').then(answer => {
      if (!answer.isConfirmed) return;
      const customerNote = {} as CustomerNote;
      customerNote.customerId = this.defaults.id;
      customerNote.createdBy = this.currentUser.userName;
      customerNote.createdDate = moment(new Date());
      customerNote.notes = this.customerNoteCtrl.value.trim();
      customerNote.isActive = true
      customerNote.isDeleted = false;
      this.customerNoteService.createCustomerNote(customerNote).subscribe({
        next: (result) => {
          if (result) {
            this.dataSource.data = result;
            this.customerNoteCtrl.setValue('');
          }
        },
        error: (e) => {
          this.alertService.failNotification("Customer Notes", "Create");
          console.error(e)
        },
        complete: () => console.info('complete') 
      });
    });
  }

  getLookups() {
    this.initializeCountryList();
    this.initializeStateList();
    this.lookupService.getCustomerTypes().subscribe((result: Lookup[]) => (this.customerTypeList = result));
    this.lookupService.getLocationTypes().subscribe((result: Lookup[]) => (this.locationTypeList = result));

    this.lookupService.getPaymentTerms().subscribe((result: PaymentTerm[]) => (this.paymentTermList = result));
    this.lookupService.getPriceLevels().subscribe((result: PriceLevel[]) => (this.priceLevelList = result));
    this.lookupService.getSalesRepresentatives().subscribe((result: SalesRepresentative[]) => (this.salesRepresentativeOutList = result.filter((obj) => { return obj.type === 0 })));
    this.lookupService.getSalesRepresentatives().subscribe((result: SalesRepresentative[]) => (this.salesRepresentativeInList = result.filter((obj) => { return obj.type === 1 })));
  }

  initializeCountryList() {
    this.countryCtrl = new UntypedFormControl();
    this.filteredCountries$ = this.countryCtrl.valueChanges.pipe(
      startWith(''),
      map(country => country ? this.filterCountries(country) : this.countryList.slice())
    );
  }

  filterCountries(name: string): any {
    return this.countryList.filter(country =>
      country.name.toLowerCase().indexOf(name.toLowerCase()) === 0)
  }

  initializeStateList() {
    this.stateCtrl = new UntypedFormControl();
    this.filteredStates$ = this.stateCtrl.valueChanges.pipe(
      startWith(''),
      map(state => state ? this.filterStates(state) : this.stateList.slice())
    );
  }

  filterStates(code: string) {
    return this.stateList.filter(state =>
      state.code.toLowerCase().indexOf(code.toLowerCase()) === 0);
  }

  onCustomerTypeChange($event) {
    // console.log(this.form.value.customerType);
    // console.log($event);
  }

  onCountrySelectionChange(name: string) {
    this.selectedCountry = this.countryList.find(c => c.name.toLowerCase() === name.toLowerCase());
    this.stateList = this.selectedCountry.states;
    this.selectedState = undefined;
    this.initializeStateList();
    this.changeDetector.detectChanges();
  }

  onStateSelectionChange($event) {
    this.selectedState = undefined;
    if ($event && $event.value && $event.value.length > 0) {
      this.selectedState = this.stateList.find(s => s.code.toLowerCase() === $event.value.toLowerCase());
    }
  }

  resetCountryControl() {
    this.countryCtrl.reset();
    this.selectedCountry = undefined;
    this.stateList = [];
    this.selectedState = undefined;
    this.initializeStateList();
  }

  togglePassword() {
    if (this.visible) {
      this.inputType = 'password';
      this.visible = false;
      this.changeDetector.markForCheck();
    } else {
      this.inputType = 'text';
      this.visible = true;
      this.changeDetector.markForCheck();
    }
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  onHoldAccountClick(checked: boolean) {
    this.isHoldAccount = checked;
  }

  getLocationTypeName(value: number) {
    let result: Lookup;
    if (this.locationTypeList && this.locationTypeList.length > 0) {
      result = this.locationTypeList.find(lt => lt.id === value);
    }
    return result ? result.name : '';
  }

  save() {
    if (this.mode === 'create') {
      if (this.form.valid && this.countryCtrl.value !== null && this.stateCtrl.value !== null) {
        // Validate Email
        let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
        if (!regexp.test(this.form.value.email.toLowerCase())) {
          this.alertService.requiredNotification('Contact email ' + this.form.value.email + ' is invalid!');
          return;
        }
        this.alertService.createNotification("Customer").then(answer => {
          if (!answer.isConfirmed) return;
          this.createCustomer();
        });
      }
      else this.alertService.validationNotification("Customer");
    }
    else if (this.mode === 'update') {
      if (this.form.valid && this.countryCtrl.value !== null && this.stateCtrl.value !== null) {
        // Validate Email
        let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
        if (!regexp.test(this.form.value.email.toLowerCase())) {
          this.alertService.requiredNotification('Contact email ' + this.form.value.email + ' is invalid!');
          return;
        }
        this.alertService.updateNotification("Customer").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateCustomer();
        });
      }
      else this.alertService.validationNotification("Customer");
    }
  }

  createCustomer() {
    let customer = this.mapDataToModel();
    this.dialogRef.close(customer);
  }

  updateCustomer() {
    let customer = this.mapDataToModel();
    this.dialogRef.close(customer);
  }

  mapDataToModel(): Customer {
    const customer = {} as Customer;

    customer.accountNumber = this.isCreateMode() ? 0 : this.defaults.accountNumber;
    customer.customerName = this.form.value.customerName;
    customer.customerTypeId = this.form.value.customerTypeId;

    if (this.form.value.priceLevelId) customer.priceLevelId = this.form.value.priceLevelId;
    if (this.form.value.paymentTermId) customer.paymentTermId = this.form.value.paymentTermId;
    if (this.form.value.salesRepresentativeOutId) customer.salesRepresentativeOutId = this.form.value.salesRepresentativeOutId;
    if (this.form.value.salesRepresentativeInId) customer.salesRepresentativeInId = this.form.value.salesRepresentativeInId;

    customer.isHoldAccount = this.isHoldAccount;
    customer.creditLimit = this.form.value.creditLimit;
    customer.taxRate = this.form.value.taxRate;
    customer.overBalance = this.form.value.overBalance;
    customer.discount = this.form.value.discount;
    customer.sellersPermit = this.form.value.sellersPermit;
    customer.crossStreet = this.form.value.crossStreet;
    customer.phoneNumber = this.form.value.phoneNumber;
    customer.faxNumber = this.form.value.faxNumber;
    customer.contactName = this.form.value.contactName;
    customer.addressLine1 = this.form.value.addressLine1;
    customer.addressLine2 = this.form.value.addressLine2;
    customer.city = this.form.value.city;
    customer.state = this.stateCtrl.value; // this.form.value.state;
    customer.country = this.countryCtrl.value; // this.form.value.country;
    customer.zipCode = this.form.value.zipCode;
    customer.email = this.form.value.email;
    customer.isActive = this.isCreateMode() ? true : this.defaults.isActive;;
    customer.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    customer.createdBy = this.isCreateMode() ? "create@user.com" : this.defaults.createdBy;
    customer.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;
    customer.id = this.isCreateMode() ? 0 : this.defaults.id;
    customer.isBypassCreditLimit = this.bypassCreditLimit;

    if (this.isUpdateMode()) {
      customer.modifiedBy = "modify@user.com";
      customer.modifiedDate = moment(new Date());
    }

    return customer;
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  setBypassCreditLimit(event: any) {
    this.bypassCreditLimit = event.checked;
  }
}
