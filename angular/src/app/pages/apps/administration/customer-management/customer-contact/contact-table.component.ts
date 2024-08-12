import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, ChangeDetectorRef, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { filter, map } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { AlertService } from 'src/services/alert.service';
import { CustomerService } from 'src/services/customer.service';
import { Contact } from 'src/services/interfaces/models';
import { Location } from 'src/services/interfaces/models';
import { LookupService } from 'src/services/lookup.service';
import { ContactCreateUpdateComponent } from './contact-create-update/contact-create-update.component';
import { ContactService } from 'src/services/contact.service';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User } from 'src/services/interfaces/models';
import { LocationService } from 'src/services/location.service';

@UntilDestroy()
@Component({
  selector: 'vex-contact-table',
  templateUrl: './contact-table.component.html',
  styleUrls: ['./contact-table.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class ContactTableComponent implements OnInit, AfterViewInit {
  @Input() customerId: number;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";

  subjectContact$: ReplaySubject<Contact[]> = new ReplaySubject<Contact[]>(1);
  dataContact$: Observable<Contact[]> = this.subjectContact$.asObservable();
  contacts: Contact[];
  positionTypeList: Lookup[];
  locationList = {} as Location[];

  dataSourceContact: MatTableDataSource<Contact> | null;
  selectionContact = new SelectionModel<Contact>(true, []);
  searchCtrlContact = new UntypedFormControl()

  pageSizeOptions: number[] = [5, 10, 20, 50];
  pageSize = 10;

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  //contacts = contactsData;
  filteredContacts$ = this.route.paramMap.pipe(
    map(paramMap => paramMap.get('activeCategory')),
    map(activeCategory => {
      switch (activeCategory) {
        case 'details': {

          return this.contacts;// contactsData;
        }

        case 'starred': {
          return this.contacts; //contactsData.filter(c => c.starred);
        }

        default: {
          return [];
        }
      }
    })
  );

  contactColumns: TableColumn<Contact>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Contact Name', property: 'contactName', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Position', property: 'positionTypeId', type: 'text', visible: true },
    { label: 'Location', property: 'locationId', type: 'text', visible: true },
    { label: 'Phone Number', property: 'phoneNumber', type: 'text', visible: true },
    { label: 'Email', property: 'email', type: 'text', visible: true },
    { label: 'Email Credit', property: 'isEmailCreditMemo', type: 'image', visible: true },
    { label: 'Email Order', property: 'isEmailOrder', type: 'image', visible: true },
    { label: 'Email Statement', property: 'isEmailStatement', type: 'image', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Contact,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private locationService: LocationService,
    private contactService: ContactService,
    private lookupService: LookupService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerManagement);
    this.access = modulePermission.accessTypeId;
    
  }

  get visibleColumnsContact() {
    return this.contactColumns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit(): void {
    this.getData();

    if (this.defaults) {
      this.mode = 'update';

      this.dataSourceContact = new MatTableDataSource();
      this.dataContact$.pipe(
        filter<Contact[]>(Boolean)
      ).subscribe(contacts => {
        this.contacts = contacts;
        this.dataSourceContact.data = contacts;
      });
    } else {
      this.defaults = {} as Contact;
    }

    this.searchCtrlContact.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onContactFilterChange(value));
  }

  getData() {
    this.locationService.getLocationsByCustomerId(this.defaults.id).subscribe((result = {} as Location[]) => (this.locationList = result));
    this.contactService.getContactsByCustomerId(this.defaults.id).subscribe((result: Contact[]) => (this.subjectContact$.next(result)));
    this.lookupService.getPositionTypes().subscribe((result: Lookup[]) => (this.positionTypeList = result));
  }

  onContactFilterChange(value: string) {
    if (!this.dataSourceContact) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.dataSourceContact.filter = value;
  }



  ngAfterViewInit() {
    if (this.dataSourceContact) {
      this.dataSourceContact.paginator = this.paginator;
      this.dataSourceContact.sort = this.sort;
    }
  }


  trackByProperty<T>(column: TableColumn<T>) {
    return column.property;
  }

  createContact() {
    this.dialog.open(ContactCreateUpdateComponent, {
      height: '40%',
      width: '40%',
      data: this.customerId
    }).afterClosed().subscribe((contact = {} as Contact) => {
      if (contact) {
        this.contactService.createContact(contact).subscribe((result: Contact[]) => {
          if (result) {
            (this.subjectContact$.next(result));
            this.alertService.successNotification("Contact", "Create");
          }
          else this.alertService.failNotification("Contact", "Create");
        });
      }
    });
  }

  updateContact(contact: any) {
    this.dialog.open(ContactCreateUpdateComponent, {
      height: '40%',
      width: '40%',
      data: contact
    }).afterClosed().subscribe(updatedContact => {
      if (updatedContact) {
        this.contactService.updateContact(updatedContact).subscribe((result: Contact[]) => {
          if (result) {
            (this.subjectContact$.next(result));
            this.alertService.successNotification("Contact", "Update");
          }
          else this.alertService.failNotification("Contact", "Update");
        });
      }
    });
  }

  deleteContact() { }
  deleteContacts() { }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllContactSelected() {
    const numSelected = this.selectionContact.selected.length;
    const numRows = this.dataSourceContact.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggleContact() {
    this.isAllContactSelected() ?
      this.selectionContact.clear() :
      this.dataSourceContact.data.forEach(row => this.selectionContact.select(row));
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  getLocationName(value: number) {
    let result = {} as Location;
    if (this.locationList && this.locationList.length > 0) {
      result = this.locationList.find(l => l.id === value);
    }
    return result ? result.locationName : '';
  }

  getPositionTypeName(value: number) {
    let result: Lookup;
    if (this.positionTypeList && this.positionTypeList.length > 0) {
      result = this.positionTypeList.find(e => e.id === value);
    }
    return result ? result.name : '';
  }
}
