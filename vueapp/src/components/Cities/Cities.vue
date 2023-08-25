<template>
	<div>
		<my-input v-model="countrySearch"
							type="text"
							placeholder="Country search"></my-input>

		<my-input v-model="citySearch"
							type="text"
							placeholder="City search"></my-input>

		<my-dialog v-model:show="dialogAddCityVisible">
			<city-form :countries="countries"
								 @add="addCity" />
		</my-dialog>
		<city-list :cities="filteredCities"
							 @delete="deleteCity" />

		<my-button @click="showDialogAddCity"
							 style="margin: 15px 0">
			Add city
		</my-button>
	</div>
</template>

<script>
  import CityForm from "@/components/Cities/CityForm";
	import CityList from "@/components/Cities/CityList";

	export default {
		name: "cities",
		components: {
			CityForm, CityList
		},
		data() {
			return {
				countries: [],
				cities: [],
				dialogAddCityVisible: false,
				countrySearch: "",
				citySearch: ""
			}
		},
		methods: {
			async getListCountries() {
				const res = await fetch('https://localhost:44364/api/content/country/getList', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ search: "" })
				});

				const finalRes = await res.json();
				const countries = finalRes.countries.map(country => ({
					id: country.countryId,
					name: country.countryName
				}));

				this.countries = countries;
			},
			async getListCities() {
				const res = await fetch('https://localhost:44364/api/content/city/getList', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ search: "" })
				});

				const finalRes = await res.json();
				const cities = finalRes.cities.map(city => ({
					countryId: city.countryId,
					countryName: city.countryName,
					cityId: city.cityId,
					cityName: city.cityName
				}));

				this.cities = cities;
			},
			async addCity(city) {
				this.dialogAddCityVisible = false;
				const res = await fetch('https://localhost:44364/api/content/city/add', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({
						countryId: city.countryId,
						countryName: city.countryName,
						cityName: city.cityName
					})
				})
					.then(response => response.json())

				city.cityId = res.id;
				this.cities.push(city);
			},
			async deleteCity(city) {
				this.cities = this.cities.filter(c => c.cityId !== city.cityId);
				await fetch('https://localhost:44364/api/content/city/delete', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ Id: city.cityId })
				})
					.then(response => response.json())
			},
			showDialogAddCity() {;
				this.getListCountries();
				this.getListCities();
				this.dialogAddCityVisible = true;
			},
		},
		mounted() {
			this.getListCountries();
			this.getListCities();
		},
		computed: {
			filteredCities() {
				return this.cities
					.filter(city => city.countryName.toLowerCase().indexOf(this.countrySearch.toLowerCase()) > -1)
					.filter(city => city.cityName.toLowerCase().indexOf(this.citySearch.toLowerCase()) > -1)
			},
		},
	}
</script>

<style>
</style>