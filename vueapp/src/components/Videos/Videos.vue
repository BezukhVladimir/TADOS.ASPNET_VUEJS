<template>
	<div>
		<my-input v-model="contentSearch"
							type="text"
							placeholder="Video search"></my-input>

		<my-dialog v-model:show="dialogAddVideoVisible">
			<video-form :users="users"
								   @add="addVideo" />
		</my-dialog>
		<video-list :videos="filteredVideos"
								:users="users"
							  @delete="deleteVideo" />

		<my-button @click="showDialogAddVideo"
							 style="margin: 15px 0">
			Add video
		</my-button>
	</div>
</template>

<script>
  import VideoForm from "@/components/Videos/VideoForm";
	import VideoList from "@/components/Videos/VideoList";

	export default {
		name: "videos",
		components: {
			VideoForm, VideoList
		},
		data() {
			return {
				users: [],
				videos: [],
				dialogAddVideoVisible: false,
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
			async getListVideos() {
				const res = await fetch('https://localhost:44364/api/content/video/getList', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ contentName: "" })
				});

				const finalRes = await res.json();
				const videos = finalRes.videos.map(video => ({
					id:          video.id,
					authorId:    (video.author !== null) ? (video.author.userId) : '-1',
					authorEmail: (video.author !== null) ? (video.author.userEmail) : 'Deleted user',
					name:        video.name,
					category:    video.category,
					url:         video.url
				}));
					
				this.videos = videos.filter(video => video.authorId != '-1');
			},
			async addVideo(video) {
				this.dialogAddVideoVisible = false;
				const res = await fetch('https://localhost:44364/api/content/video/add', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({
						userId:          video.authorId,
						contentName:     video.name,
						contentCategory: video.category,
						videoURL:        video.url
					})
				})
					.then(response => response.json())

				video.id = res.id;
				this.videos.push(video);
			},
			async deleteVideo(video) {
				this.videos = this.videos.filter(v => v.id !== video.id);
				await fetch('https://localhost:44364/api/content/video/delete', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ Id: video.id })
				})
					.then(response => response.json())
			},
			showDialogAddVideo() {
				this.getListUsers();
				this.dialogAddVideoVisible = true;
			},
		},
		mounted() {
			this.getListUsers();
			this.getListVideos();
		},
		computed: {
			filteredVideos() {
				return this.videos
					.filter(video => video.name.toLowerCase().indexOf(this.contentSearch.toLowerCase()) > -1)
			},
		},
	}
</script>

<style>
</style>