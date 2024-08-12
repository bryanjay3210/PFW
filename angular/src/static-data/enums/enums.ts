export enum ModuleGroupCode {
    Administration = 'ADM',
    PurchasingManagement = 'PCM',
    ReportManagement = 'RPM',
    SystemMaintenance = 'SYS',

    // Administration	ADM	Administration Module Group
    // System Maintenance	SYS	System Maintenance Module Group
    // Settings	STN	Settings
    // Demo Testing	DMO	DMO
    // Customer Service	CSM	Customer Service
    // Report Management	RPM	Report Management Group
    // Purchasing Management	PCM	Purchasing Management Module
}

export enum ModuleCode {
    // Administration	ADM	Administration Module Group
    CustomerManagement = 'CMM',
    ProductManagement = 'PMM',
    UserManagement	= 'UMM',
    RolesAndPermissions	= 'RPM',
    ItemMasterlistReference = 'IRM',
    VendorCatalogMasterlist = 'VCM',
    OrderManagement	= 'COM',
    PaymentManagement = 'PYM',
    WarehouseManagement	= 'WHM',

    // System Maintenance	SYS	System Maintenance Module Group
    ModuleManagement = 'MMM',
    SettingsMaintenance = 'SMM',

    // Report Management	RPM	Report Management Group
    StatementReport = 'SAR',
    InvoiceReport = 'IVM',
    CustomerSalesReport = 'CSR',

    // Purchasing Management	PCM	Purchasing Management Module
    VendorManagement = 'VMM',
    PurchaseOrderManagement = 'POM',

    // Warehouse Management
    BinLocationPutAway = 'BLM',
    PartsPicking = 'PPM',
    DriverLog = 'DLM',
    StockSettings = 'SSM',
    PartsManifest = 'PRM',

    // External Customer
    CustomerOrderManagement = 'EOM',

    EmailModule = 'EMM'



    // Customer Management	CMM	Customer Management Module
    // Product Management	PMM	Product Management Module
    // Inventory Management	IMM	Inventory Management Module
    // User Management	UMM	User Management Module
    // Roles & Permissions	RPM	Roles & Permissions Management Module
    // Module Management	MMM	Module Management Module
    // Settings Maintenance	SMM	Settings Maintenance
    // Test Module	TST	Test Module
    // Demo Module	DMO	Demo Module
    // Inventory Management 2	IM2	Inventory Management 2
    // Category	CAT	Parts Category
    // Item Masterlist Reference	IRM	Item Masterlist Reference
    // Vendor Catalog Masterlist	VCM	Vendor Catalog Management
    // Order Management	COM	Customer Order Management
    // Payment Management	PYM	Payment Management and Accounting
    // Warehouse Management	WHM	Warehouse Management Module
    // Statement Report	SAR	Statement of Accounts Report
    // Vendor Management	VMM	Vendor Management Module
    // Purchase Order Management	POM	Purchase Order Management Module
    // Invoice Report	IVM	Invoice Report Module
}    

export enum UserPermission {
    NoAccess = 1,
    ViewAccess = 2,
    FullAccess = 3
}

export enum OrderStatus {
    NewOrder = 1,
    OnHold = 2,
    Closed = 3,
    Cancelled = 4
}

export enum PartsFilter {
    Year = 1,
    Make = 2,
    Model = 3,
    Category = 4,
    Sequence = 5
}

export enum StoreLocation {
    California = 'CA',
    Nevada = 'NV'
}

export enum DeliveryRoute {
    AM = 1,
    PM = 2
}

// locationList = [ {id: 'CA', code: 'CA'}, {id: 'NV', code: 'NV'} ];
// deliveryRouteList = [ {id: 1, code: 'AM'}, {id: 2, code: 'PM'} ];