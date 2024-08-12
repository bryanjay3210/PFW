import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs/internal/BehaviorSubject";
import { Observable } from "rxjs/internal/Observable";
import { User } from "./interfaces/models";
import { ModuleGroup } from "./interfaces/moduleGroup.model";
import { ModuleGroupService } from "./modulegroup.service";

@Injectable({
    providedIn: 'root'
})
export class MenuService {
    // Public Properties
    menuList$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);
    moduleGroups: ModuleGroup[] = []
    filteredMenuItems: any[] = [];
    filteredModules: any[] = [];
    user$ =  Observable<User>;
    _user: any;
    userId: any;
    _userRoleAccess;
    currentUser = {} as User;
    

    constructor(
        private moduleGroupService: ModuleGroupService,

    ) {
        this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
        this.loadMenu();
    }

    loadMenu() {
        this.currentUser.role.rolePermissions.forEach(rp => {
            if (rp.accessTypeId !== 1) {
                // console.log (rp.moduleGroup.name + ' - ' + rp.module.name);
            }
        });
    }
}