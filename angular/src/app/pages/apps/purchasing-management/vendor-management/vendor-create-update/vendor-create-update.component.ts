import { ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { scaleFadeIn400ms } from 'src/@vex/animations/scale-fade-in.animation';
import { scaleIn400ms } from 'src/@vex/animations/scale-in.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { trackById } from 'src/@vex/utils/track-by';
import moment from 'moment';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { UntilDestroy } from '@ngneat/until-destroy';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { AlertService } from 'src/services/alert.service';
import { countries, Country } from 'src/static-data/country-data';
import { State } from 'src/static-data/state-data';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User, Vendor } from 'src/services/interfaces/models';


export interface CountryState {
  name: string;
  population: string;
  flag: string;
}

@UntilDestroy()
@Component({
  selector: 'vex-vendor-create-update',
  templateUrl: './vendor-create-update.component.html',
  styleUrls: ['./vendor-create-update.component.scss'],
  animations: [
    scaleIn400ms,
    fadeInRight400ms,
    stagger40ms,
    fadeInUp400ms,
    scaleFadeIn400ms
  ]
})

export class VendorCreateUpdateComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";
  pageSizeOptions: number[] = [5, 10, 20, 50];
  pageSize = 10;
  static id = 100;
  visible = false;

  trackById = trackById;
  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  countryList: Country[] = countries;
  stateList: State[] = undefined;
  selectedCountry: Country = undefined;
  selectedState: State = undefined;

  countryCtrl: UntypedFormControl;
  filteredCountries$: Observable<Country[]>;
  stateCtrl: UntypedFormControl;
  filteredStates$: Observable<State[]>;

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  isCAVendor: boolean = false;
  isNVVendor: boolean = false;

  cutoffTime = new FormControl(null, Validators.required);
  focused = false;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Vendor,
    private dialogRef: MatDialogRef<VendorCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private alertService: AlertService,
    private changeDetector: ChangeDetectorRef) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.VendorManagement);
    this.access = modulePermission.accessTypeId;
  }

  getErrorMessage() {
    return "Cutoff time is required."
  }

  ngOnInit() {
    this.getLookups();

    if (this.defaults) {
      this.mode = 'update';
      this.isCAVendor = this.defaults.isCAVendor;
      this.isNVVendor = this.defaults.isNVVendor;
      this.cutoffTime.setValue(moment(this.defaults.cutoffTime).format("HH:mm"));
      this.selectedCountry = this.countryList.find(c => c.name === this.defaults.country);

      // console.log(moment(this.defaults.cutoffTime).format("HH:mm")); // 24 hour format
      // console.log(moment(this.defaults.cutoffTime).format("hh:mm a")); // use 'A' for uppercase AM/PM
      // console.log(moment(this.defaults.cutoffTime).format("hh:mm:ss A")); // with milliseconds

      if (this.selectedCountry) this.stateList = this.selectedCountry.states;
      this.selectedState = this.stateList.find(s => s.code === this.defaults.state);
      this.stateCtrl.setValue(this.defaults.state);
    } else {
      this.defaults = {} as Vendor;
      this.selectedCountry = this.countryList.find(c => c.code === 'US');
      if (this.selectedCountry) this.stateList = this.selectedCountry.states;
      this.selectedState = undefined;
    }

    this.countryCtrl.setValue(this.selectedCountry.name);

    this.form = this.fb.group({
      id: [VendorCreateUpdateComponent.id++],
      vendorName: [this.defaults.vendorName || '', Validators.required],
      vendorCode: [this.defaults.vendorCode || '', Validators.required],
      contactName: [this.defaults.contactName || '', Validators.required],
      phoneNumber: [this.defaults.phoneNumber || '', Validators.required],
      faxNumber: [this.defaults.faxNumber || '', Validators.required],
      addressLine1: [this.defaults.addressLine1 || '', Validators.required],
      addressLine2: [this.defaults.addressLine2 || ''],
      city: [this.defaults.city || '', Validators.required],
      state: [this.defaults.state || ''],
      country: [this.defaults.country || this.selectedCountry.name, Validators.required],
      zipCode: [this.defaults.zipCode || '', Validators.required],
      email: [this.defaults.email || '', Validators.required],
      caRank: [this.defaults.caRank || 0],
      nvRank: [this.defaults.nvRank || 0],
      caPercentage: [this.defaults.caPercentage || 0],
      nvPercentage: [this.defaults.nvPercentage || 0]
      // cutoffTime: [this.defaults.cutoffTime || 0]
    });
  }

  getLookups() {
    this.initializeCountryList();
    this.initializeStateList();
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

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(column: TableColumn<T>) {
    return column.property;
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  save() {
    if (this.mode === 'create') {
      if (this.form.valid && this.countryCtrl.value !== null && this.stateCtrl.value !== null) {
        if (this.cutoffTime.value === null || this.cutoffTime.value === '') {
          this.alertService.validationNotification("Vendor");
          return;
        }

        if (!this.ValidRanking()) return;

        this.alertService.createNotification("Vendor").then(answer => {
          if (!answer.isConfirmed) return;
          this.createVendor();
        });
      }
      else this.alertService.validationNotification("Vendor");
    }
    else if (this.mode === 'update') {
      if (this.form.valid && this.countryCtrl.value !== null && this.stateCtrl.value !== null) {
        if (this.cutoffTime.value === null || this.cutoffTime.value === '') {
          this.alertService.validationNotification("Vendor");
          return;
        }
        
        if (!this.ValidRanking()) return;

        this.alertService.updateNotification("Vendor").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateVendor();
        });
      }
      else this.alertService.validationNotification("Vendor");
    }
  }

  ValidRanking(): boolean {
    if (this.isCAVendor && (!this.form.value.caRank || this.form.value.caRank === 0)) {
      this.alertService.requiredNotification("CA Vendor Rank is required.");
      return false;

    }

    if (this.isCAVendor && (!this.form.value.caPercentage || this.form.value.caPercentage === 0)) {
      this.alertService.requiredNotification("CA Vendor Percentage is required.");
      return false;
    }

    if (this.isNVVendor && (!this.form.value.nvRank || this.form.value.nvRank === 0)) {
      this.alertService.requiredNotification("NV Vendor Rank is required.");
      return false;
    }

    if (this.isNVVendor && (!this.form.value.nvPercentage || this.form.value.nvPercentage === 0)) {
      this.alertService.requiredNotification("NV Vendor Percentage is required.");
      return false;
    }

    return true;
  }

  createVendor() {
    const vendor = {} as Vendor; 
    this.mapDataToModel(vendor);
    this.dialogRef.close(vendor);
  }

  updateVendor() {
    const vendor = {} as Vendor; 
    this.mapDataToModel(vendor);
    this.dialogRef.close(vendor);
  }

  mapDataToModel(vendor: Vendor) {
    vendor.vendorName = this.form.value.vendorName;
    vendor.vendorCode = this.form.value.vendorCode;
    vendor.contactName = this.form.value.contactName;
    vendor.phoneNumber = this.form.value.phoneNumber;
    vendor.faxNumber = this.form.value.faxNumber;
    vendor.addressLine1 = this.form.value.addressLine1;
    vendor.addressLine2 = this.form.value.addressLine2;
    vendor.city = this.form.value.city;
    vendor.state = this.stateCtrl.value; // this.form.value.state;
    vendor.country = this.countryCtrl.value; // this.form.value.country;
    vendor.zipCode = this.form.value.zipCode;
    vendor.email = this.form.value.email;
    vendor.isActive = this.isCreateMode() ? true : this.defaults.isActive;;
    vendor.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    vendor.createdBy = this.isCreateMode() ? "create@user.com" : this.defaults.createdBy;
    vendor.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;
    
    vendor.isCAVendor = this.isCAVendor;
    vendor.isNVVendor = this.isNVVendor;
    
    if (this.isCAVendor) {
      vendor.caRank = this.form.value.caRank;
      vendor.caPercentage = this.form.value.caPercentage;
    }
    else {
      vendor.caRank = 0;
      vendor.caPercentage = 0;
    }
    
    if (this.isNVVendor) {
      vendor.nvRank = this.form.value.nvRank;
      vendor.nvPercentage = this.form.value.nvPercentage;
    }
    else {
      vendor.nvRank = 0;
      vendor.nvPercentage = 0;
    }
    
    vendor.id = this.isCreateMode() ? 0 : this.defaults.id;

    if (this.isUpdateMode()) {
      vendor.modifiedBy = this.currentUser.userName;
      vendor.modifiedDate = moment(new Date());
    }

    let minDate = '0001-01-01' + ' ' + this.cutoffTime.value;
    vendor.cutoffTime = moment(new Date(minDate));

    return vendor;
  }

  setCAVendor(event: any) {
    this.isCAVendor = event.checked;
  }

  setNVVendor(event: any) {
    this.isNVVendor = event.checked;
  }
}
