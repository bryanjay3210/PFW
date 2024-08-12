import { SelectionModel } from '@angular/cdk/collections';
import { ChangeDetectorRef, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { filter, map } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { AlertService } from 'src/services/alert.service';
import { CustomerService } from 'src/services/customer.service';
import { Module } from 'src/services/interfaces/module.model';
import { LookupService } from 'src/services/lookup.service';
import { ModuleCreateUpdateComponent } from './module-create-update/module-create-update.component';
import { ModuleService } from 'src/services/module.service';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { ModuleGroup } from 'src/services/interfaces/moduleGroup.model';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User } from 'src/services/interfaces/models';

@UntilDestroy()
@Component({
  selector: 'vex-module-table',
  templateUrl: './module-table.component.html',
  styleUrls: ['./module-table.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class ModuleTableComponent implements OnInit {
  @Input() moduleGroup: ModuleGroup;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";
  
  subject$: ReplaySubject<Module[]> = new ReplaySubject<Module[]>(1);
  data$: Observable<Module[]> = this.subject$.asObservable();
  modules: Module[];
  modulesList: Module[];
  
  dataSource: MatTableDataSource<Module> | null;
  selection = new SelectionModel<Module>(true, []);
  searchCtrl = new UntypedFormControl()

  pageSizeOptions: number[] = [5, 10, 20, 50];
  pageSize = 10;
  
  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  
  //modules = modulesData;
  filteredModules$ = this.route.paramMap.pipe(
    map(paramMap => paramMap.get('activeCategory')),
    map(activeCategory => {
      switch (activeCategory) {
        case 'details': {
          
          return this.modules;// modulesData;
        }

        case 'starred': {
          return this.modules; //modulesData.filter(c => c.starred);
        }

        default: {
          return [];
        }
      }
    })
  );
  
  tableColumns: TableColumn<Module>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Module Name', property: 'name', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Code', property: 'code', type: 'text', visible: true },
    { label: 'Description', property: 'description', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];
  
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess
  
  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Module,
  private router: Router,
  private dialogRef: MatDialogRef<ModuleCreateUpdateComponent>,
  private fb: UntypedFormBuilder,
  private dialog: MatDialog,
  private route: ActivatedRoute,
  private moduleService: ModuleService,
  private customerService: CustomerService,
  private lookupService: LookupService,
  private alertService: AlertService,
  private changeDetector: ChangeDetectorRef) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ModuleManagement);
    this.access = modulePermission.accessTypeId;
   }

  get visibleColumns() {
    return this.tableColumns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit(): void {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.getData();

    if (this.defaults) {
      this.mode = 'update';

      this.dataSource = new MatTableDataSource();
      this.data$.pipe(
        filter<Module[]>(Boolean)
      ).subscribe(modules => {
        this.modules = modules;
        this.dataSource.data = modules;
      });
    } else {
      this.defaults = {} as Module;
    }

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));    
  }

  getData() {
    this.subject$.next(this.moduleGroup.modules);
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.dataSource.filter = value;
  }

  ngAfterViewInit() {
    if (this.dataSource){
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  createRecord() { 
    this.dialog.open(ModuleCreateUpdateComponent, {
      data: this.moduleGroup.id
    }).afterClosed().subscribe((module: Module) => {
      if (module) {
        this.moduleService.createModule(module).subscribe((result: Module[]) => { 
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Module", "Create");
          }
          else this.alertService.failNotification("Module", "Create");
        });
      }
    });
  }

  updateRecord(module: any) {
    this.dialog.open(ModuleCreateUpdateComponent, {
      data: module
    }).afterClosed().subscribe(updatedModule => {
      if (updatedModule) {
        this.moduleService.updateModule(updatedModule).subscribe((result: Module[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Module", "Update");
          }
          else this.alertService.failNotification("Module", "Update");
        });
      }
    });
   }

  deleteRecord(module: any) { }
  deleteRecords(module: any) { }

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

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
