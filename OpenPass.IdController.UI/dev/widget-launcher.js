const rollup = require('rollup');
const terser = require('rollup-plugin-terser').terser;
const replace = require('@rollup/plugin-replace');
const typescript = require('@rollup/plugin-typescript');

const [, , ...params] = process.argv;
const isBuild = params.includes('--build');

const defaultConfig = {
  input: 'projects/widget-launcher/src/main.ts',
  output: getOutputConfig('projects/widget/src/assets/'),
  plugins: [
    typescript({
      tsconfig: 'projects/widget-launcher/tsconfig.json',
    }),
  ],
};

let configs = [defaultConfig];
if (isBuild) {
  configs = [
    {
      ...defaultConfig,
      output: getOutputConfig('dist/widget/assets/'),
      plugins: [replace({ 'environment/environment': 'environment/environment.prod' }), ...defaultConfig.plugins],
    },
  ];
}

function getOutputConfig(path) {
  return [
    {
      file: path + 'widget.js',
      format: 'iife',
    },
    {
      file: path + 'widget.min.js',
      format: 'iife',
      plugins: [terser()],
    },
  ];
}

async function build(config) {
  const bundle = await rollup.rollup(config);
  config.output.forEach((output) => bundle.write(output));

  return bundle.close();
}

configs.forEach(build);
