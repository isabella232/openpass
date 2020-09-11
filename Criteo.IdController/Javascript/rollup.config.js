import typescript from 'rollup-plugin-typescript2';

function generateRollupConfig(tsInput, jsOutput) {
    return {
        input: tsInput,
        output: {
            file: jsOutput,
            format: 'esm'
        },
        plugins: [
            typescript(/*{ plugin options }*/)
        ]
    }
}

const widgetConfig = generateRollupConfig('./src/views/widget.ts', '../wwwroot/js/widget.js');
const initialBannerConfig = generateRollupConfig('./src/views/initialBanner.ts', '../wwwroot/js/initialBanner.js');
const optionsBannerConfig = generateRollupConfig('./src/views/optionsBanner.ts', '../wwwroot/js/optionsBanner.js');
const learnMoreSiteConfig = generateRollupConfig('./src/views/learnMoreSite.ts', '../wwwroot/js/learnMoreSite.js');

export default [
    widgetConfig,
    initialBannerConfig,
    optionsBannerConfig,
    learnMoreSiteConfig
]