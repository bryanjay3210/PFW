import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { LocationService } from 'src/services/location.service';
import { LocationDTO, User } from 'src/services/interfaces/models';
import { filter } from 'rxjs/operators';
import { Observable } from 'rxjs/internal/Observable';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@UntilDestroy()
@Component({
  selector: 'vex-location-list',
  templateUrl: './location-list.component.html',
  styleUrls: ['./location-list.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class LocationListComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";

  subject$: ReplaySubject<LocationDTO[]> = new ReplaySubject<LocationDTO[]>(1);
  data$: Observable<LocationDTO[]> = this.subject$.asObservable();
  locations: LocationDTO[];
  dataSource: MatTableDataSource<LocationDTO> | null;
  selection = new SelectionModel<LocationDTO>(true, []);
  searchCtrl = new UntypedFormControl()

  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  columns: TableColumn<LocationDTO>[] = [
    { label: 'Location Name', property: 'locationName', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Location Code', property: 'locationCode', type: 'text', visible: false},
    { label: 'Location Type', property: 'locationTypeId', type: 'text', visible: true },
    { label: 'Address', property: 'addressLine1', type: 'text', visible: true },
    { label: 'Address 2', property: 'addressLine2', type: 'text', visible: true },
    { label: 'City', property: 'city', type: 'text', visible: true },
    { label: 'State', property: 'state', type: 'text', visible: true },
    { label: 'Country', property: 'country', type: 'text', visible: false },
    { label: 'ZipCode', property: 'zipCode', type: 'text', visible: true },
    { label: 'Zone', property: 'zone', type: 'text', visible: true },
    { label: 'Contact Name', property: 'contactName', type: 'text', visible: true },
    { label: 'Phone Number', property: 'phoneNumber', type: 'text', visible: true },
    { label: 'Fax Number', property: 'faxNumber', type: 'text', visible: false },
    { label: 'Email', property: 'email', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }

    // zone?: string | undefined;
  ];

  constructor(
    @Inject(MAT_DIALOG_DATA) public customerId: number,
    private dialogRef: MatDialogRef<LocationListComponent>,
    private router: Router,
    private locationService: LocationService,
    private alertService: AlertService
  ) { 
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit(): void {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.getData();

    this.dataSource = new MatTableDataSource();
    this.data$.pipe(
      filter<LocationDTO[]>(Boolean)
    ).subscribe(locations => {
      this.locations = locations;
      this.dataSource.data = locations;
    });

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  getData() {
    this.alertService.showBlockUI('Loading Locations...');
    this.locationService.getLocationsList(this.customerId).subscribe((result: LocationDTO[]) => {
      this.subject$.next(result);
      this.alertService.hideBlockUI();
    });
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

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  selectLocation(location: LocationDTO){
    this.dialogRef.close(location);
  }
}
