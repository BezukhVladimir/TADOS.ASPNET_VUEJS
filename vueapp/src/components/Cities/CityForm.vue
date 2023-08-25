<template>
	<div>
		<form @submit.prevent>
			<h3>Add city</h3>
			<my-select :options="countries"
								 :selected="selectedCountry"
								 @select="countrySelect">
			</my-select>
			<my-input v-model="city.cityName"
								type="text"
								placeholder="Name"></my-input>
			<my-button style="align-self: flex-end; margin-top: 15px"
								 @click="addCity">
				Add!
			</my-button>
		</form>
	</div>
</template>

<script>
	export default {
		data() {
			return {
				city: {
					countryId: '',
					countryName: '',
					cityId: '',
					cityName: ''
				},
				hasCountrySelected: false,
				selectedCountry: {
					id: '',
					name: 'Select country'
				}
			}
		},
		props: {
			countries: {
				type: Array,
				required: true
			}
		},
		methods: {
			countrySelect(option) {
				this.selectedCountry = option;
				this.hasCountrySelected = true;
			},
			addCity() {
				if (this.hasCountrySelected) {
					this.city = {
						countryId: this.selectedCountry.id,
						countryName: this.selectedCountry.name,
						cityId: '',
						cityName: this.city.cityName
					}
					this.$emit('add', this.city)
					this.hasCountrySelected = false;
				}
			}
		}
	}
</script>

<style>
	form {
		display: flex;
		flex-direction: column;
	}
</style>