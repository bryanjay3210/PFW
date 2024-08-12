import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { Lookup } from "./interfaces/lookup.model";
import { PaymentTerm } from "./interfaces/paymentTerm.model";
import { PriceLevel } from "./interfaces/priceLevel.model";
import { SalesRepresentative } from "./interfaces/salesRepresentative.model";
import { environment } from "src/environments/environment";
import { CategoryDTO, MakeDTO, ModelDTO, PaymentType, ProductFilterDTO, SequenceDTO, Warehouse, WarehousePartDTO, YearDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class LookupService {
    url = "Lookup"
    constructor(private http: HttpClient) {}

    public getCustomerTypes() : Observable<Lookup[]> {
        return this.http.get<Lookup[]>(`${environment.apiUrl}/${this.url}/GetCustomerTypes`);
    }

    public getLocationTypes() : Observable<Lookup[]> {
        return this.http.get<Lookup[]>(`${environment.apiUrl}/${this.url}/GetLocationTypes`);
    }

    public getPositionTypes() : Observable<Lookup[]> {
        return this.http.get<Lookup[]>(`${environment.apiUrl}/${this.url}/GetPositionTypes`);
    }

    public getUserTypes() : Observable<Lookup[]> {
        return this.http.get<Lookup[]>(`${environment.apiUrl}/${this.url}/GetUserTypes`);
    }

    public getAccessTypes() : Observable<Lookup[]> {
        return this.http.get<Lookup[]>(`${environment.apiUrl}/${this.url}/GetAccessTypes`);
    }

    public getPaymentTypes() : Observable<PaymentType[]> {
        return this.http.get<PaymentType[]>(`${environment.apiUrl}/${this.url}/GetPaymentTypes`);
    }

    public getPriceLevels() : Observable<PriceLevel[]> {
        return this.http.get<PriceLevel[]>(`${environment.apiUrl}/${this.url}/GetPriceLevels`);
    }

    public getPaymentTerms() : Observable<PaymentTerm[]> {
        return this.http.get<PaymentTerm[]>(`${environment.apiUrl}/${this.url}/GetPaymentTerms`);
    }

    public getSalesRepresentatives() : Observable<SalesRepresentative[]> {
        return this.http.get<SalesRepresentative[]>(`${environment.apiUrl}/${this.url}/GetSalesRepresentatives`);
    }

    public getWarehouses() : Observable<Warehouse[]> {
        return this.http.get<Warehouse[]>(`${environment.apiUrl}/${this.url}/GetWarehouses`);
    }

    public getWarehousePartsByProductId(productId: number) : Observable<WarehousePartDTO[]> {
        return this.http.get<WarehousePartDTO[]>(`${environment.apiUrl}/${this.url}/GetWarehousePartsByProductId?productId=${productId}`);
    }

    public getYearsListDistinct() : Observable<YearDTO[]> {
        return this.http.get<YearDTO[]>(`${environment.apiUrl}/${this.url}/GetYearsListDistinct`);
    }

    public getMakesListDistinct() : Observable<MakeDTO[]> {
        return this.http.get<MakeDTO[]>(`${environment.apiUrl}/${this.url}/GetMakesListDistinct`);
    }

    public getModelsListDistinct() : Observable<ModelDTO[]> {
        return this.http.get<ModelDTO[]>(`${environment.apiUrl}/${this.url}/GetModelsListDistinct`);
    }

    public getCategoriesListDistinct() : Observable<CategoryDTO[]> {
        return this.http.get<CategoryDTO[]>(`${environment.apiUrl}/${this.url}/GetCategoriesListDistinct`);
    }

    public getSequencesListDistinct() : Observable<SequenceDTO[]> {
        return this.http.get<SequenceDTO[]>(`${environment.apiUrl}/${this.url}/GetSequencesListDistinct`);
    }

    public getMakesListByYear(productFilterDTO: ProductFilterDTO) : Observable<MakeDTO[]> {
        return this.http.get<MakeDTO[]>(`${environment.apiUrl}/${this.url}/GetMakesListByYear?year=${productFilterDTO.year}&make=${productFilterDTO.make}&model=${productFilterDTO.model}&categoryIds=${productFilterDTO.categoryIds}&sequenceIds=${productFilterDTO.sequenceIds}`);
    }

    public getModelsListByMake(productFilterDTO: ProductFilterDTO) : Observable<ModelDTO[]> {
        return this.http.get<ModelDTO[]>(`${environment.apiUrl}/${this.url}/GetModelsListByMake?year=${productFilterDTO.year}&make=${productFilterDTO.make}&model=${productFilterDTO.model}&categoryIds=${productFilterDTO.categoryIds}&sequenceIds=${productFilterDTO.sequenceIds}`);
    }

    public getCategoriesListByModel(productFilterDTO: ProductFilterDTO) : Observable<CategoryDTO[]> {
        return this.http.get<CategoryDTO[]>(`${environment.apiUrl}/${this.url}/GetCategoriesListByModel?year=${productFilterDTO.year}&make=${productFilterDTO.make}&model=${productFilterDTO.model}&categoryIds=${productFilterDTO.categoryIds}&sequenceIds=${productFilterDTO.sequenceIds}`);
    }

    public getSequencesListByCategoryId(productFilterDTO: ProductFilterDTO) : Observable<SequenceDTO[]> {
        return this.http.get<SequenceDTO[]>(`${environment.apiUrl}/${this.url}/GetSequencesListByCategoryId?year=${productFilterDTO.year}&make=${productFilterDTO.make}&model=${productFilterDTO.model}&categoryIds=${productFilterDTO.categoryIds}&sequenceIds=${productFilterDTO.sequenceIds}`);
    }
}