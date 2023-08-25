<template>
	<div>
		<my-input v-model="contentSearch"
							type="text"
							placeholder="Image search"></my-input>

		<my-dialog v-model:show="dialogAddImageVisible">
			<image-form :users="users"
								  @add="addImage" />
		</my-dialog>
		<image-list :images="filteredImages"
								:users="users"
							  @delete="deleteImage" />

		<my-button @click="showDialogAddImage"
							 style="margin: 15px 0">
			Add image
		</my-button>
	</div>
</template>

<script>
  import ImageForm from "@/components/Images/ImageForm";
	import ImageList from "@/components/Images/ImageList";

	export default {
		name: "images",
		components: {
			ImageForm, ImageList
		},
		data() {
			return {
				users: [],
				images: [],
				dialogAddImageVisible: false,
				contentSearch: ""
			}
		},
		methods: {
			async getListUsers() {
				const res = await fetch('https://localhost:44364/api/content/user/getList', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ search: "" })
				});

				const finalRes = await res.json();
				const users = finalRes.users.map(user => ({
					countryId:   user.countryId,
					countryName: user.countryName,
					cityId:      user.cityId,
					cityName:    user.cityName,
					userId:      user.userId,
					userEmail:   user.userEmail
				}));

				this.users = users;
			},
			async getListImages() {
				const res = await fetch('https://localhost:44364/api/content/image/getList', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ contentName: "" })
				});

				const finalRes = await res.json();
				const images = finalRes.images.map(image => ({
					id:          image.id,
					authorId:    (image.author !== null) ? (image.author.userId) : '-1',
					authorEmail: (image.author !== null) ? (image.author.userEmail) : 'Deleted user',
					name:        image.name,
					category:    image.category,
					url:         image.url
				}));
					
				this.images = images.filter(image => image.authorId != '-1');
			},
			async addImage(image) {
				this.dialogAddImageVisible = false;
				const res = await fetch('https://localhost:44364/api/content/image/add', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({
						userId:          image.authorId,
						contentName:     image.name,
						contentCategory: image.category,
						imageURL:        image.url
					})
				})
					.then(response => response.json())

				image.id = res.id;
				this.images.push(image);
			},
			async deleteImage(image) {
				this.images = this.images.filter(i => i.id !== image.id);
				await fetch('https://localhost:44364/api/content/image/delete', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ Id: image.id })
				})
					.then(response => response.json())
			},
			showDialogAddImage() {
				this.getListUsers();
				this.dialogAddImageVisible = true;
			},
		},
		mounted() {
			this.getListUsers();
			this.getListImages();
		},
		computed: {
			filteredImages() {
				return this.images
					.filter(image => image.name.toLowerCase().indexOf(this.contentSearch.toLowerCase()) > -1)
			},
		},
	}
</script>

<style>
</style>