<template>
	<div>
		<my-input v-model="countrySearch"
							type="text"
							placeholder="Country search"></my-input>

		<my-dialog v-model:show="dialogAddCountryVisible">
			<country-form @add="addCountry" />
		</my-dialog>

		<country-list :countries="filteredCountries"
									@delete="deleteCountry" />

		<my-button @click="showDialogAddCountry"
							 style="margin: 15px 0">
			Add country
		</my-button>
	</div>
</template>

<script>
	import CountryForm from "@/components/Countries/CountryForm";
	import CountryList from "@/components/Countries/CountryList";

	export default {
		name: "countries",
		components: {
			CountryForm, CountryList
		},
		data() {
			return {
				countries: [],
				dialogAddCountryVisible: false,
				countrySearch: ''
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
			async addCountry(country) {
				this.dialogAddCountryVisible = false;
				const res = await fetch('https://localhost:44364/api/content/country/add', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ countryName: country.name })
				})
					.then(response => response.json())

				country.id = res.id;
				this.countries.push(country);
			},
			async deleteCountry(country) {
				this.countries = this.countries.filter(c => c.id !== country.id);
				await fetch('https://localhost:44364/api/content/country/delete', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ Id: country.id })
				})
					.then(response => response.json())
			},
			showDialogAddCountry() {
				this.getListCountries();
				this.dialogAddCountryVisible = true;
			},
		},
		mounted() {
			this.getListCountries();
		},
		computed: {
			filteredCountries() {
				return this.countries
					.filter(country => country.name.toLowerCase().indexOf(this.countrySearch.toLowerCase()) > -1)
			},
		},
	}
</script>

<style>
</style>