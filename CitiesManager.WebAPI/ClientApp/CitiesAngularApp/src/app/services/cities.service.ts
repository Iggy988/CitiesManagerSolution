import { Injectable } from '@angular/core';
import { City } from "../models/city";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class CitiesService {
  cities: City[] = [];
  constructor(private httpClient: HttpClient) {
    //this.cities = [
    //  new City("101", "New York"),
    //  new City("102", "London"),
    //  new City("103", "Berlin"),
    //  new City("104", "Paris"),
    //]
  }

  public getCities(): Observable<City[]> {
    //return this.cities;
    return this.httpClient.get<City[]>("https://localhost:7187/api/v1/cities")
  }
}
