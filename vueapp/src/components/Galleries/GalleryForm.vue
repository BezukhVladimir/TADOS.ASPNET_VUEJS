<template>
	<div>
		<form @submit.prevent>
			<h3>Add gallery</h3>
			<my-select :options="users"
								 :selected="selectedUser"
								 @select="userSelect">
			</my-select>

			<my-select :options="images"
								 :selected="selectedCoverImage"
								 @select="coverImageSelect">
			</my-select>

			<my-input v-model="gallery.name"
								type="text"
								placeholder="Name"></my-input>

			<div class="select">
				<select v-model="selectedImages" multiple>
					<option v-for="image in images"
									v-bind:value="image">
						{{ image.name }}
					</option>
				</select>

				<ul>
					<li v-for="image in selectedImages">
						{{ image.name }}
					</li>
				</ul>
			</div>

			<my-button style="align-self: flex-end; margin-top: 15px"
								 @click="addGallery">
				Add!
			</my-button>
		</form>
	</div>
</template>

<script>
	export default {
		data() {
			return {
				gallery: {
					id: '',
					authorId: '',
					name: '',
					category: 'Gallery',
					cover: {
						id: '',
						authorId: '',
						name: '',
						category: '',
						url: ''
					},
					images: []
				},
				hasUserSelected: false,
				selectedUser: {
					countryId: '',
					countryName: '',
					cityId: '',
					citiName: '',
					userId: '',
					userEmail: 'Select user'
				},
				hasCoverImageSelected: false,
				selectedCoverImage: {
					id: '',
					authorId: '',
					name: '',
					category: '',
					url: 'Select cover image'
				},
				selectedImages: []
			}
		},
		props: {
			users: {
				type: Array,
				required: true
			},
			images: {
				type: Array,
				required: true
			},
		},
		methods: {
			userSelect(option) {
				this.selectedUser = option;
				this.hasUserSelected = true;
			},
			coverImageSelect(option) {
				this.selectedCoverImage = option;
				this.hasCoverImageSelected = true;
			},
			addGallery() {
				if (this.hasUserSelected) {
					if (this.hasCoverImageSelected) {
						this.gallery = {
							id: '',
							authorId:    this.selectedUser.userId,
							authorEmail: this.selectedUser.userEmail,
							name:        this.gallery.name,
							category:    this.gallery.category,
							cover:       this.selectedCoverImage,
							images:      this.selectedImages
						}

						this.$emit('add', this.gallery)
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

	.select {
		margin-top: 15px;
	}
</style>