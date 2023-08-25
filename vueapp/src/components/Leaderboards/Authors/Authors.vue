<template>
	<div>
		<author-list :authors="authors"/>
	</div>
</template>

<script>
	import AuthorList from "@/components/Leaderboards/Authors/AuthorList";

	export default {
		name: "authors",
		components: {
			AuthorList
		},
		data() {
			return {
				authors: []
			}
		},
		methods: {
			async getListAuthors() {
				const res = await fetch('https://localhost:44364/api/content/leaderboard/author/getTop', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ count: 10 })
				});

				const finalRes = await res.json();
				const authors = finalRes.authors.map(author => ({
					id:          author.id,
					email:       author.email,
					averageRate: author.averageRate
				}));

				this.authors = authors;
			}
		},
		mounted() {
			this.getListAuthors();
		}
	}
</script>

<style>
</style>