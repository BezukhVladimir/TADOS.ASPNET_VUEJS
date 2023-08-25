<template>
	<div>
		<form @submit.prevent>
			<h3>Add article</h3>
			<my-select :options="users"
								 :selected="selectedUser"
								 @select="userSelect">
			</my-select>
			<my-input v-model="article.name"
								type="text"
								placeholder="Name"></my-input>
			<my-input v-model="article.text"
								type="text"
								placeholder="Text"></my-input>
			<my-button style="align-self: flex-end; margin-top: 15px"
								 @click="addArticle">
				Add!
			</my-button>
		</form>
	</div>
</template>

<script>
	export default {
		data() {
			return {
				article: {
					id: '',
					authorId: '',
					name: '',
					category: 'Article',
					text: '',
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
			addArticle() {
				if (this.hasUserSelected) {
					this.article = {
						id: '',
						authorId:    this.selectedUser.userId,
						authorEmail: this.selectedUser.userEmail,
						name:        this.article.name,
						category:    this.article.category,
						text:        this.article.text						
					}
					this.$emit('add', this.article)
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