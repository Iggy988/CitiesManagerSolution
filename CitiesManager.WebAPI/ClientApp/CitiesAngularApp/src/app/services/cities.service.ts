import { Injectable } from '@angular/core';
import { City } from "../models/city";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";

const API_BASE_URL: string = "https://localhost:7187/api/";

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

    let headers = new HttpHeaders();
    headers = headers.append("Authorization", "Bearer mytoken");

    return this.httpClient.get<City[]>(`${API_BASE_URL}v1/cities`, { headers: headers });
  }

  public postCities(city: City): Observable<City> {
    //return this.cities;

    let headers = new HttpHeaders();
    headers = headers.append("Authorization", "Bearer mytoken");

    return this.httpClient.post<City>(`${API_BASE_URL}v1/cities`, city, { headers: headers });
  }
}
