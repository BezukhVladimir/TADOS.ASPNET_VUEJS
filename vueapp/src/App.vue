<template>
	<div class="app">
		<h1>Content App</h1>

		<div class="wrap">
			<div class="table">
				<my-tabs>
					<my-tab title="Countries">
						<countries />
					</my-tab>
					<my-tab title="Cities">
						<cities />
					</my-tab>
					<my-tab title="Users">
						<users />
					</my-tab>
				</my-tabs>
			</div>

			<div class="table">
				<my-tabs>
					<my-tab title="Images">
						<images />
					</my-tab>
					<my-tab title="Galleries">
						<galleries />
					</my-tab>
					<my-tab title="Videos">
						<videos />
					</my-tab>
					<my-tab title="Articles">
						<articles />
					</my-tab>
				</my-tabs>
			</div>

			<div class="table">
				<my-tabs>
					<my-tab title="Top content">
						<contents />
					</my-tab>
					<my-tab title="Top author">
						<authors />
					</my-tab>
				</my-tabs>
			</div>
		</div>
	</div>
</template>

<script>
	import Countries from "@/components/Countries/Countries";
	import Cities    from "@/components/Cities/Cities";
	import Users     from "@/components/Users/Users";
	import Images    from "@/components/Images/Images";
	import Videos    from "@/components/Videos/Videos";
	import Articles  from "@/components/Articles/Articles";
	import Galleries from "@/components/Galleries/Galleries";
	import Contents  from "@/components/Leaderboards/Contents/Contents";
	import Authors   from "@/components/Leaderboards/Authors/Authors";

	export default {
		components: {
			Countries,
			Cities,
			Users,
			Images,
			Videos,
			Articles,
			Galleries,
			Contents,
			Authors
		},
	}
</script>

<style lang="css">
	* {
		margin: 0;
		padding: 0;
		box-sizing: border-box;
	}

	.app {
		padding: 15px;
	}

	.wrap {
		max-width: 1200px;
		max-height: 3000px;
	}

	.table {
		display: inline-flex;
	}
</style>