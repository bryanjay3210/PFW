import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { SelectionModel } from '@angular/cdk/collections';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { UntypedFormControl } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { ItemMasterlistReferenceService } from 'src/services/itemmasterlistreference.service';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { ItemMasterlistReference, User } from 'src/services/interfaces/models';
import { ItemMasterlistReferenceCreateUpdateComponent } from './item-masterlist-reference-create-update/item-masterlist-reference-create-update.component';

@UntilDestroy()
@Component({
  selector: 'vex-item-masterlist-reference-table',
  templateUrl: './item-masterlist-reference-table.component.html',
  styleUrls: ['./item-masterlist-reference-table.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ],
  providers: [
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: {
        appearance: 'standard'
      } as MatFormFieldDefaultOptions
    }
  ]
})

export class ItemMasterlistReferenceTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<ItemMasterlistReference>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Part Number', property: 'partNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Parts Link Number', property: 'partsLinkNumber', type: 'text', visible: true },
    { label: 'OEM Number', property: 'oemNumber', type: 'text', visible: true },
    { label: 'Main Parts Link', property: 'isMainPartsLink', type: 'text', visible: true },
    { label: 'Main OEM', property: 'isMainOEM', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');

  subject$: ReplaySubject<ItemMasterlistReference[]> = new ReplaySubject<ItemMasterlistReference[]>(1);
  data$: Observable<ItemMasterlistReference[]> = this.subject$.asObservable();
  inventories: ItemMasterlistReference[];

  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  dataSource: MatTableDataSource<ItemMasterlistReference> | null;
  selection = new SelectionModel<ItemMasterlistReference>(true, []);
  searchCtrl = new UntypedFormControl();

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private itemMasterlistReferenceService: ItemMasterlistReferenceService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ItemMasterlistReference);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.getData();

    this.dataSource = new MatTableDataSource();

    this.data$.pipe(
      filter<ItemMasterlistReference[]>(Boolean)
    ).subscribe(inventories => {
      this.inventories = inventories;
      this.dataSource.data = inventories;
    });

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  getData() {
    this.alertService.showBlockUI('Loading Item Masterlist...');
    this.itemMasterlistReferenceService.getItemMasterlistReferences().subscribe((result: ItemMasterlistReference[]) => {
      this.subject$.next(result);
      this.alertService.hideBlockUI();
    });
  }

  createItemMasterlistReference() {
    this.dialog.open(ItemMasterlistReferenceCreateUpdateComponent, {
      // height: '90%',
      // width: '60%',
    }).afterClosed().subscribe((itemMasterlistReference: ItemMasterlistReference) => {
      if (itemMasterlistReference) {
        this.itemMasterlistReferenceService.createItemMasterlistReference(itemMasterlistReference).subscribe((result: ItemMasterlistReference[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Item Masterlist Reference", "Create");
          }
          else {
            this.alertService.failNotification("Item Masterlist Reference", "Create");
          }
        });
      }
    });
  }

  updateItemMasterlistReference(itemMasterlistReference: ItemMasterlistReference) {
    this.dialog.open(ItemMasterlistReferenceCreateUpdateComponent, {
      // height: '90%',
      // width: '60%',
      data: itemMasterlistReference
    }).afterClosed().subscribe(updatedItemMasterlistReference => {
      if (updatedItemMasterlistReference) {
        this.itemMasterlistReferenceService.updateItemMasterlistReference(updatedItemMasterlistReference).subscribe((result: ItemMasterlistReference[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Item Masterlist Reference", "Update");
          }
          else this.alertService.failNotification("Item Masterlist Reference", "Update");
        });
      }
    });
  }

  deleteItemMasterlistReference(itemMasterlistReference: ItemMasterlistReference) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (itemMasterlistReference) {
      // this.userService.deleteItemMasterlistReference([user]).subscribe((result: ItemMasterlistReference[]) => (this.subject$.next(result)));
    }

    // this.inventories.splice(this.inventories.findIndex((existingItemMasterlistReference) => existingItemMasterlistReference.id === user.id), 1);
    // this.selection.deselect(user);
    // this.subject$.next(this.inventories);
  }

  deleteItemMasterlistReferences(inventories: ItemMasterlistReference[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deleteItemMasterlistReference(inventories).subscribe((result: ItemMasterlistReference[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deleteItemMasterlistReference(c));
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

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }
}
