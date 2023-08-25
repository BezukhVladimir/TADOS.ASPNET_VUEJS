<template>
	<div class="my-select">
		<p
			 class="title"
			 @click="optionsVisible = !optionsVisible"
		>
			{{ selected }}
		</p>
		<div
				 class="options"
				 v-if="optionsVisible"
			>
			<p
				v-for="option in options"
				:key="option.value"
				@click="optionSelect(option)"
			>
				{{ option }}
			</p>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'my-select',
		props: {
			options: {
				type: Array,
				default() {
					return []
				}
			},
			selected: {
				type: String,
				default: ''
			}
		},
		data() {
			return {
				optionsVisible: false
			}
		},
		methods: {
			optionSelect(option) {
				this.$emit('select', option);
				this.optionsVisible = false;
			},
			hideSelect() {
				this.optionsVisible = false;
			}
		},
		mounted() {
			document.addEventListener('click', this.hideSelect.bind(this), true);
		},
		beforeDestroy() {
			document.removeEventListener('click', this.hideSelect);
		}
	}
</script>

<style>
	.my-select {
		position: relative;
		min-height: 50px;
		min-width: 300px;
		cursor: pointer;
		margin-top: 10px;
	}
	.my-select p {
		margin: 0;
	}

	.title {
		border: 2px solid dodgerblue;
		vertical-align: middle;
		font-family: Helvetica, Arial, sans-serif;
		font-size: 12px;
		color: rgba(0, 0, 0, 0.6);
		padding: 10px 15px;
	}

	.options {
		width: 100%;
		border: 2px solid dodgerblue;
		font-family: Helvetica, Arial, sans-serif;
		font-size: 12px;
		color: rgba(0, 0, 0, 0.6);
		padding: 10px 15px;
		margin-top: 10px;
	}

	.options p:hover {
		background: #e7e7e7;
	}
</style>