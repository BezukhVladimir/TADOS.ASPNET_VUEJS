<template>
	<div>
		<form @submit.prevent>
			<h3>Add user</h3>
			<my-select :options="cities"
								 :selected="selectedCity"
								 @select="citySelect">
			</my-select>
			<my-input v-model="user.userEmail"
								type="text"
								placeholder="Email"></my-input>
			<my-button style="align-self: flex-end; margin-top: 15px"
								 @click="addUser">
				Add!
			</my-button>
		</form>
	</div>
</template>

<script>
	export default {
		data() {
			return {
				user: {
					countryId: '',
					countryName: '',
					cityId: '',
					cityName: '',
					userId: '',
					userEmail: ''
				},
				hasCitySelected: false,
				selectedCity: {
					countryId: '',
					countryName: '',
					cityId: '',
					cityName: 'Select city'
				}
			}
		},
		props: {
			cities: {
				type: Array,
				required: true
			}
		},
		methods: {
			citySelect(option) {
				this.selectedCity = option;
				this.hasCitySelected = true;
			},
			addUser() {
				if (this.hasCitySelected) {
					this.user = {
						countryId:   this.selectedCity.countryId,
						countryName: this.selectedCity.countryName,
						cityId:      this.selectedCity.cityId,
						cityName:    this.selectedCity.cityName,
						userId: '',
						userEmail:   this.user.userEmail
					}
					this.$emit('add', this.user)
					this.hasCitySelected = false;
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