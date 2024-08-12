import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { LookupService } from 'src/services/lookup.service';
import { countries, Country } from 'src/static-data/country-data';
import { State } from 'src/static-data/state-data';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User } from 'src/services/interfaces/models';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { Location } from 'src/services/interfaces/models';

@Component({
  selector: 'vex-location-create-update',
  templateUrl: './location-create-update.component.html',
  styleUrls: ['./location-create-update.component.scss']
})
export class LocationCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  customerId: number;
  locationTypeList: Lookup[];

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

  constructor(
    @Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<LocationCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private changeDetector: ChangeDetectorRef,
    private lookupService: LookupService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    this.initializeCountryList();
    this.initializeStateList();
    this.lookupService.getLocationTypes().subscribe((result: Lookup[]) => (this.locationTypeList = result));

    if (this.defaults && isNaN(this.defaults)) {
      this.mode = 'update';
      this.selectedCountry = this.countryList.find(c => c.name === this.defaults.country);
      if (this.selectedCountry) this.stateList = this.selectedCountry.states;
      this.selectedState = this.stateList.find(s => s.code === this.defaults.state);
      this.stateCtrl.setValue(this.defaults.state);
    } else {
      this.customerId = this.defaults;
      this.defaults = {} as Location;
      this.selectedCountry = this.countryList.find(c => c.code === 'US');
      if (this.selectedCountry) this.stateList = this.selectedCountry.states;
      this.selectedState = undefined;
    }

    this.countryCtrl.setValue(this.selectedCountry.name);

    this.form = this.fb.group({
      id: [LocationCreateUpdateComponent.id++],
      locationName: [this.defaults.locationName || '', Validators.required],
      locationCode: [this.defaults.locationCode || '', Validators.required],
      locationTypeId: [this.defaults.locationTypeId || undefined, Validators.required],

      phoneNumber: [this.defaults.phoneNumber || '', Validators.required],
      faxNumber: [this.defaults.faxNumber || '', Validators.required],
      addressLine1: [this.defaults.addressLine1 || '', Validators.required],
      addressLine2: [this.defaults.addressLine2 || ''],
      city: [this.defaults.city || undefined, Validators.required],
      state: [this.defaults.state || ''],
      country: [this.defaults.country || this.selectedCountry.name, Validators.required],
      zipCode: [this.defaults.zipCode || '', Validators.required],
      email: [this.defaults.email || ''],
      notes: this.defaults.notes
    });

    this.changeDetector.detectChanges();
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

  save() {
    if (this.mode === 'create') {
      if (this.form.valid && this.countryCtrl.value !== null && this.stateCtrl.value !== null) {
        this.alertService.createNotification("Location").then(answer => {
          if (!answer.isConfirmed) return;
          this.createLocation();
        });
      }
      else this.alertService.validationNotification("Location");
    }
    else if (this.mode === 'update') {
      if (this.form.valid && this.countryCtrl.value !== null && this.stateCtrl.value !== null) {
        this.alertService.updateNotification("Location").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateLocation();
        });
      }
      else this.alertService.validationNotification("Location");
    }
  }

  createLocation() {
    const location = {} as Location;
    this.mapFormValuesToInterface(location);
    this.dialogRef.close(location);
  }

  mapFormValuesToInterface(location: Location) {
    location.customerId = this.isCreateMode() ? this.customerId : this.defaults.customerId;
    location.locationTypeId = this.form.value.locationTypeId;
    location.locationName = this.form.value.locationName;
    location.locationCode = this.form.value.locationCode;
    location.phoneNumber = this.form.value.phoneNumber;
    location.faxNumber = this.form.value.faxNumber;
    location.email = this.form.value.email;
    location.notes = this.form.value.notes;

    location.addressLine1 = this.form.value.addressLine1;
    location.addressLine2 = this.form.value.addressLine2;
    location.country = this.countryCtrl.value;
    location.state = this.stateCtrl.value;
    location.city = this.form.value.city;
    location.zipCode = this.form.value.zipCode;

    location.isActive = this.isCreateMode() ? true : this.defaults.isActive;
    location.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    location.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
    location.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;

    if (this.isUpdateMode()) {
      location.modifiedBy = this.currentUser.userName;
      location.modifiedDate = moment(new Date());
      location.id = this.defaults.id;
    }
  }

  updateLocation() {
    const location = {} as Location;
    this.mapFormValuesToInterface(location);
    this.dialogRef.close(location);
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  onCountryChange($event) {
    this.selectedCountry = this.countryList.find(c => c.name === $event.value);
    this.stateList = this.selectedCountry.states;
    this.selectedState = undefined;
    this.changeDetector.detectChanges();
  }

  onStateChange($event) {
    this.selectedState = this.stateList.find(s => s.code === $event.value);
  }
}
