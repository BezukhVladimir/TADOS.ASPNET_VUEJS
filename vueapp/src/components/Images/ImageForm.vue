<template>
	<div>
		<form @submit.prevent>
			<h3>Add image</h3>
			<my-select :options="users"
								 :selected="selectedUser"
								 @select="userSelect">
			</my-select>
			<my-input v-model="image.name"
								type="text"
								placeholder="Name"></my-input>
			<my-input v-model="image.url"
								type="text"
								placeholder="URL"></my-input>
			<my-button style="align-self: flex-end; margin-top: 15px"
								 @click="addImage">
				Add!
			</my-button>
		</form>
	</div>
</template>

<script>
	export default {
		data() {
			return {
				image: {
					id: '',
					authorId: '',
					name: '',
					category: 'Image',
					url: '',
				},
				hasUserSelected: false,
				selectedUser: {
					countryId: '',
					countryName: '',
					cityId: '',
					citiName: '',
					userId: '',
					userEmail: 'Select user'
				}
			}
		},
		props: {
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
			addImage() {
				if (this.hasUserSelected) {
					this.image = {
						id: '',
						authorId:    this.selectedUser.userId,
						authorEmail: this.selectedUser.userEmail,
						name:        this.image.name,
						category:    this.image.category,
						url:         this.image.url						
					}
					this.$emit('add', this.image)
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