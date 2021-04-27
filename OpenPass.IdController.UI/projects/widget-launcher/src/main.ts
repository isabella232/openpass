import { requireScript } from './utils/require-script';
import { environment } from './environment/environment';

environment.scriptNames.forEach(requireScript);
