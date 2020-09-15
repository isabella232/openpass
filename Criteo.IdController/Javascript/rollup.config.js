import typescript from 'rollup-plugin-typescript2';
import replace from 'rollup-plugin-replace';


function generateRollupConfig(tsInput, jsOutput, isDevEnv) {
    return {
        input: tsInput,
        output: {
            file: jsOutput,
            format: 'esm'
        },
        plugins: [
            typescript(/*{ plugin options }*/),
            replace({
                '__MACRO-CONTROLLER-URI__': isDevEnv ? 'http://localhost:1234' : 'https://id-controller.crto.in'
            })
        ]
    };
}

const isDevEnv = process.env.CRITEO_ENV === 'dev';

const widgetConfig = generateRollupConfig('./src/views/widget.ts', '../wwwroot/js/widget.js', isDevEnv);
const initialBannerConfig = generateRollupConfig('./src/views/initialBanner.ts', '../wwwroot/js/initialBanner.js', isDevEnv);
const optionsBannerConfig = generateRollupConfig('./src/views/optionsBanner.ts', '../wwwroot/js/optionsBanner.js', isDevEnv);
const learnMoreSiteConfig = generateRollupConfig('./src/views/learnMoreSite.ts', '../wwwroot/js/learnMoreSite.js', isDevEnv);

export default [
    widgetConfig,
    initialBannerConfig,
    optionsBannerConfig,
    learnMoreSiteConfig
]

