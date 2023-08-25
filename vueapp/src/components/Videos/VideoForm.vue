<template>
	<div>
		<form @submit.prevent>
			<h3>Add video</h3>
			<my-select :options="users"
								 :selected="selectedUser"
								 @select="userSelect">
			</my-select>
			<my-input v-model="video.name"
								type="text"
								placeholder="Name"></my-input>
			<my-input v-model="video.url"
								type="text"
								placeholder="URL"></my-input>
			<my-button style="align-self: flex-end; margin-top: 15px"
								 @click="addVideo">
				Add!
			</my-button>
		</form>
	</div>
</template>

<script>
	export default {
		data() {
			return {
				video: {
					id: '',
					authorId: '',
					name: '',
					category: 'Video',
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
			addVideo() {
				if (this.hasUserSelected) {
					this.video = {
						id: '',
						authorId:    this.selectedUser.userId,
						authorEmail: this.selectedUser.userEmail,
						name:        this.video.name,
						category:    this.video.category,
						url:         this.video.url						
					}
					this.$emit('add', this.video)
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