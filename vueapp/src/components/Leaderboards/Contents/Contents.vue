<template>
	<div>
		<content-list :contents="contents"/>
	</div>
</template>

<script>
	import ContentList from "@/components/Leaderboards/Contents/ContentList";

	export default {
		name: "contents",
		components: {
			ContentList
		},
		data() {
			return {
				contents: []
			}
		},
		methods: {
			async getListContents() {
				const res = await fetch('https://localhost:44364/api/content/leaderboard/content/getTop', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ count: 10 })
				});

				const finalRes = await res.json();
				const contents = finalRes.contents.map(content => ({
					id:          content.id,
					name:        content.name,
					category:    content.category,
					averageRate: content.averageRate
				}));

				this.contents = contents;
			}
		},
		mounted() {
			this.getListContents();
		}
	}
</script>

<style>
</style>