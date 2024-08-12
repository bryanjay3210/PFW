import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { Product, ProductDTO, ProductDTOPaginatedListDTO, ProductFilterDTO, ProductListDTOPaginatedListDTO, ProductPaginatedListDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class ProductService {
    url = "Product"
    constructor(private http: HttpClient) {}

    public getProductById(productId: number) : Observable<ProductDTO> {
        return this.http.get<ProductDTO>(`${environment.apiUrl}/${this.url}/GetProductById?productId=${productId}`);
    }

    public getProductByIdAndState(productId: number, state: number) : Observable<ProductDTO> {
        return this.http.get<ProductDTO>(`${environment.apiUrl}/${this.url}/GetProductByIdAndState?productId=${productId}&state=${state}`);
    }

    public getProductByIdNoStocks(productId: number) : Observable<ProductDTO> {
        return this.http.get<ProductDTO>(`${environment.apiUrl}/${this.url}/GetProductByIdNoStocks?productId=${productId}`);
    }

    public getProductByPartNumber(partNumber: string) : Observable<Product[]> {
        return this.http.get<Product[]>(`${environment.apiUrl}/${this.url}/GetProductByPartNumber?partNumber=${partNumber}`);
    }

    public getProductInLocationByPartNumber(warehouseLocationId: number, partNumber: string) : Observable<Product[]> {
        return this.http.get<Product[]>(`${environment.apiUrl}/${this.url}/GetProductInLocationByPartNumber?warehouseLocationId=${warehouseLocationId}&partNumber=${partNumber}`);
    }

    public getProducts() : Observable<Product[]> {
        return this.http.get<Product[]>(`${environment.apiUrl}/${this.url}/GetProducts`);
    }

    public getSingleProduct(searchKey: string) : Observable<Product> {
        return this.http.get<Product>(`${environment.apiUrl}/${this.url}/GeSearch`);
    }

    public getProductsPaginated(isIncludeInactive: boolean, pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<ProductPaginatedListDTO> {
        return this.http.get<ProductPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetProductsPaginated?isIncludeInactive=${isIncludeInactive}&pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getProductsList(take: number, position: number) : Observable<ProductDTO[]> {
        return this.http.get<ProductDTO[]>(`${environment.apiUrl}/${this.url}/GetProductsList`);
    }

    public getProductsListFiltered(productFilterDTO: ProductFilterDTO) : Observable<ProductDTO[]> {
        return this.http.put<ProductDTO[]>(`${environment.apiUrl}/${this.url}/GetProductsListFiltered`, productFilterDTO);
    }

    public getSearchProductsListByYearMakeModelPaginated(productFilterDTO: ProductFilterDTO, pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<ProductListDTOPaginatedListDTO> {
        return this.http.get<ProductListDTOPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetSearchProductsListByYearMakeModelPaginated?state=${productFilterDTO.state}&year=${productFilterDTO.year}&make=${productFilterDTO.make}&model=${productFilterDTO.model}&categoryIds=${productFilterDTO.categoryIds}&sequenceIds=${productFilterDTO.sequenceIds}&pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getSearchProductsListByPartNumberPaginated(state: number, pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<ProductListDTOPaginatedListDTO> {
        return this.http.get<ProductListDTOPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetSearchProductsListByPartNumberPaginated?state=${state}&pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }


    public getProductsListFilteredPaginated(productFilterDTO: ProductFilterDTO, pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<ProductDTOPaginatedListDTO> {
        return this.http.get<ProductDTOPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetProductsListFilteredPaginated?year=${productFilterDTO.year}&make=${productFilterDTO.make}&model=${productFilterDTO.model}&categoryIds=${productFilterDTO.categoryIds}&sequenceIds=${productFilterDTO.sequenceIds}&pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getProductsListByPartNumberPaginated(partNumber: string, pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<ProductDTOPaginatedListDTO> {
        return this.http.get<ProductDTOPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetProductsListByPartNumberPaginated?partNumber=${partNumber}&pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public createProduct(product: Product) : Observable<Product[]> {
        return this.http.post<Product[]>(`${environment.apiUrl}/${this.url}/CreateProduct`, product);
    }

    public updateProduct(product: Product) : Observable<Product[]> {
        return this.http.put<Product[]>(`${environment.apiUrl}/${this.url}/UpdateProduct`, product);
    }

    public deleteProduct(products: Product[]) : Observable<Product[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: products.map(a => a.id),
          };
          
        return this.http.delete<Product[]>(`${environment.apiUrl}/${this.url}/DeleteProduct`, options);
    }
}
