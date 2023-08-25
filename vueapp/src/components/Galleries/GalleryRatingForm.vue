<template>
	<div>
		<form @submit.prevent>
			<h3>Add rating</h3>
			<my-select :options="users"
								 :selected="selectedUser"
								 @select="userSelect">
			</my-select>

			<my-select :options="rates"
								 :selected="selectedRate"
								 @select="rateSelect">
			</my-select>

			<my-button style="align-self: flex-end; margin-top: 15px"
								 @click="rateGallery">
				Rate!
			</my-button>
		</form>
	</div>
</template>

<script>
	export default {
		data() {
			return {
				rating: {
					id: '',
					raterUserId: '',
					ratedContentId: '',
					rate: -1,
				},
				rates: [
					1, 2, 3, 4, 5
				],
				selectedRate: 'Choose a rating',
				hasRateSelected: false,
				selectedUser: {
					countryId: '',
					countryName: '',
					cityId: '',
					citiName: '',
					userId: '',
					userEmail: 'Select rater user'
				},
				hasUserSelected: false,
			}
		},
		props: {
			gallery: {
				type: Object,
				required: true
			},
			users: {
				type: Array,
				required: true
			}
		},
		methods: {
			userSelect(option) {
				this.selectedUser = option;
				this.hasUserSelected = true;
			},
			rateSelect(option) {
				this.selectedRate = option;
				this.hasRateSelected = true;
			},
			rateGallery() {
				if (this.hasUserSelected) {
					if (this.hasRateSelected) {
						this.rating = {
							id: '',
							raterUserId:    this.selectedUser.userId,
							ratedContentId: this.gallery.id,
							rate:           this.selectedRate,
						}

						this.$emit('rate', this.rating)
					}
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