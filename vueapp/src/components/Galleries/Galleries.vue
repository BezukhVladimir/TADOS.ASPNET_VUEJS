<template>
	<div>
		<my-input v-model="contentSearch"
							type="text"
							placeholder="Gallery search"></my-input>

		<my-dialog v-model:show="dialogAddGalleryVisible">
			<gallery-form :users="users"
										:images="images"
								    @add="addGallery" />
		</my-dialog>

		<gallery-list :galleries="filteredGalleries"
									:users="users"
							    @delete="deleteGallery" />

		<my-button @click="showDialogAddGallery"
							 style="margin: 15px 0">
			Add gallery
		</my-button>
	</div>
</template>

<script>
  import GalleryForm from "@/components/Galleries/GalleryForm";
	import GalleryList from "@/components/Galleries/GalleryList";

	export default {
		name: "galleries",
		components: {
			GalleryForm, GalleryList
		},
		data() {
			return {
				users: [],
				images: [],
				galleries: [],
				dialogAddGalleryVisible: false,
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
			async getListGalleries() {
				const res = await fetch('https://localhost:44364/api/content/image_gallery/getList', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ contentName: "" })
				});

				const finalRes = await res.json();
				const galleries = finalRes.imageGalleries.map(gallery => ({
					id:          gallery.id,
					authorId:    (gallery.author !== null) ? (gallery.author.userId)    : '-1',
					authorEmail: (gallery.author !== null) ? (gallery.author.userEmail) : 'Deleted user',
					name:        gallery.name,
					category:    gallery.category,
					cover:       gallery.cover,
					images:      gallery.images.map(image => ({
											 	 id:          image.id,
												 authorId:    (image.author !== null) ? (image.author.userId) : '-1',
												 authorEmail: (image.author !== null) ? (image.author.userEmail) : 'Deleted user',
												 name:        image.name,
												 category:    image.category,
											   url:         image.url
											 })).filter(image => image.authorId != '-1')			
				}));

				this.galleries = galleries.filter(gallery => gallery.authorId != '-1');
			},
			async addGallery(gallery) {
				const ids = [];
				gallery.images.forEach(image => ids.push(image.id));

				this.dialogAddGalleryVisible = false;
				const res = await fetch('https://localhost:44364/api/content/image_gallery/add', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({
						userId:          gallery.authorId,
						contentName:     gallery.name,
						contentCategory: gallery.category,
						coverImageId:    gallery.cover.id,
						imageIds:				 ids	
					})
				})
					.then(response => response.json())

				gallery.id = res.id;
				this.galleries.push(gallery);
			},
			async deleteGallery(gallery) {
				this.galleries = this.galleries.filter(g => g.id !== gallery.id);
				await fetch('https://localhost:44364/api/content/image_gallery/delete', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ Id: gallery.id })
				})
					.then(response => response.json())
			},
			showDialogAddGallery() {
				this.getListUsers();
				this.getListImages();
				this.dialogAddGalleryVisible = true;
			},
		},
		mounted() {
			this.getListUsers();
			this.getListImages();
			this.getListGalleries();
		},
		computed: {
			filteredGalleries() {
				return this.galleries
					.filter(gallery => gallery.name.toLowerCase().indexOf(this.contentSearch.toLowerCase()) > -1)
			},
		},
	}
</script>

<style>
</style>