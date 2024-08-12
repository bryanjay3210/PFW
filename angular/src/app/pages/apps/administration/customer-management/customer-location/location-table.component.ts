import { SelectionModel } from '@angular/cdk/collections';
import { ChangeDetectorRef, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { filter, map, startWith } from 'rxjs/operators';
import { Observable } from 'rxjs/internal/Observable';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { AlertService } from 'src/services/alert.service';
import { CustomerService } from 'src/services/customer.service';
import { Location } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { LocationService } from 'src/services/location.service';
import { LookupService } from 'src/services/lookup.service';
import { countries, Country } from 'src/static-data/country-data';
import { State } from 'src/static-data/state-data';
import { LocationCreateUpdateComponent } from '../customer-location/location-create-update/location-create-update.component';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ActivatedRoute } from '@angular/router';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User } from 'src/services/interfaces/models';

@UntilDestroy()
@Component({
  selector: 'vex-location-table',
  templateUrl: './location-table.component.html',
  styleUrls: ['./location-table.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class LocationTableComponent implements OnInit {
  @Input() customerId: number;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";

  subjectLocation$: ReplaySubject<Location[]> = new ReplaySubject<Location[]>(1);
  dataLocation$: Observable<Location[]> = this.subjectLocation$.asObservable();
  locations = {} as Location[];

  pageSizeOptions: number[] = [5, 10, 20, 50];
  pageSize = 10;

  dataSourceLocation: MatTableDataSource<Location> | null;
  selectionLocation = new SelectionModel<Location>(true, []);
  searchCtrlLocation = new UntypedFormControl();

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  countryList: Country[] = countries;
  stateList: State[] = undefined;
  locationTypeList: Lookup[];
  locationList = {} as Location[];

  selectedCountry: Country = undefined;
  selectedState: State = undefined;

  countryCtrl: UntypedFormControl;
  filteredCountries$: Observable<Country[]>;
  stateCtrl: UntypedFormControl;
  filteredStates$: Observable<State[]>;

  locationColumns: TableColumn<Location>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Location Name', property: 'locationName', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Location Code', property: 'locationCode', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Location Type', property: 'locationTypeId', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Address Line 1', property: 'addressLine1', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Address Line 2', property: 'addressLine2', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'City', property: 'city', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'State', property: 'state', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Country', property: 'country', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Zip Code', property: 'zipCode', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Phone Number', property: 'phoneNumber', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Fax Number', property: 'faxNumber', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Email', property: 'email', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Latitude', property: 'latitude', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Longitude', property: 'longitude', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Notes', property: 'notes', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  constructor(@Inject(MAT_DIALOG_DATA) public defaults = {} as Location,
    private dialogRef: MatDialogRef<LocationCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private locationService: LocationService,
    private customerService: CustomerService,
    private lookupService: LookupService,
    private alertService: AlertService,
    private changeDetector: ChangeDetectorRef) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerManagement);
    this.access = modulePermission.accessTypeId;
  }

  get visibleColumnsLocation() {
    return this.locationColumns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit(): void {
    this.getLookups();

    if (this.defaults) {
      this.mode = 'update';
      this.selectedCountry = this.countryList.find(c => c.name === this.defaults.country);

      if (this.selectedCountry) this.stateList = this.selectedCountry.states;
      this.selectedState = this.stateList.find(s => s.code === this.defaults.state);
      this.stateCtrl.setValue(this.defaults.state);

      this.getData();

      this.dataSourceLocation = new MatTableDataSource();
      this.dataLocation$.pipe(
        filter<Location[]>(Boolean)
      ).subscribe(locations => {
        this.locations = locations;
        this.dataSourceLocation.data = locations;
      });
    } else {
      this.defaults = {} as Location;
      this.selectedCountry = this.countryList.find(c => c.code === 'US');
      if (this.selectedCountry) this.stateList = this.selectedCountry.states;
      this.selectedState = undefined;
    }

    this.searchCtrlLocation.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onLocationFilterChange(value));
  }

  ngAfterViewInit() {
    if (this.dataSourceLocation) {
      this.dataSourceLocation.paginator = this.paginator;
      this.dataSourceLocation.sort = this.sort;
    }
  }

  getData() {
    this.locationService.getLocationsByCustomerId(this.defaults.id).subscribe((result = {} as Location[]) => (this.subjectLocation$.next(result)));
    this.locationService.getLocationsByCustomerId(this.defaults.id).subscribe((result = {} as Location[]) => (this.locationList = result));
  }

  getLookups() {
    this.initializeCountryList();
    this.initializeStateList();
    // this.lookupService.getCustomerTypes().subscribe((result: Lookup[]) => (this.customerTypeList = result));
    this.lookupService.getLocationTypes().subscribe((result: Lookup[]) => (this.locationTypeList = result));
    // this.lookupService.getPositionTypes().subscribe((result: Lookup[]) => (this.positionTypeList = result));
  }

  initializeStateList() {
    this.stateCtrl = new UntypedFormControl();
    this.filteredStates$ = this.stateCtrl.valueChanges.pipe(
      startWith(''),
      map(state => state ? this.filterStates(state) : this.stateList.slice())
    );
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

  filterStates(code: string) {
    return this.stateList.filter(state =>
      state.code.toLowerCase().indexOf(code.toLowerCase()) === 0);
  }

  onLocationFilterChange(value: string) {
    if (!this.dataSourceLocation) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.dataSourceLocation.filter = value;
  }

  getLocationTypeName(value: number) {
    let result: Lookup;
    if (this.locationTypeList && this.locationTypeList.length > 0) {
      result = this.locationTypeList.find(lt => lt.id === value);
    }
    return result ? result.name : '';
  }

  createLocation() {
    this.dialog.open(LocationCreateUpdateComponent, {
      data: this.customerId
    }).afterClosed().subscribe((location = {} as Location) => {
      if (location) {
        this.locationService.createLocation(location).subscribe((result = {} as Location[]) => {
          if (result) {
            (this.subjectLocation$.next(result))
            this.alertService.successNotification("Location", "Create");
          }
          else this.alertService.failNotification("Location", "Create");
        });
      }
    });
  }

  updateLocation(location: any) {
    this.dialog.open(LocationCreateUpdateComponent, {
      data: location
    }).afterClosed().subscribe(updatedLocation => {
      if (updatedLocation) {
        this.locationService.updateLocation(updatedLocation).subscribe((result = {} as Location[]) => {
          if (result) {
            (this.subjectLocation$.next(result));
            this.alertService.successNotification("Location", "Update");
          }
          else this.alertService.failNotification("Location", "Update");
        });
      }
    });
  }

  deleteLocation(location: any) { }
  deleteLocations(locations: any) { }


  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  isAllLocationSelected() {
    const numSelected = this.selectionLocation.selected.length;
    const numRows = this.dataSourceLocation.data.length;
    return numSelected === numRows;
  }

  masterToggleLocation() {
    this.isAllLocationSelected() ?
      this.selectionLocation.clear() :
      this.dataSourceLocation.data.forEach(row => this.selectionLocation.select(row));
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
